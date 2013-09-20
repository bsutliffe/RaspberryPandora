using RaspberryPandora.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using RaspberryPandora.GStreamer;
using System.Threading;

namespace RaspberryPandora {
	public class Player {
		private APIWrapper api;
		GStreamerPlayer gStreamer = new GStreamerPlayer();

		private int currentStation = 0;
		private int playlistPosition = -1;
		private int bufferPosition = -1;
		private List<Song> playlist = new List<Song>();
		private Station[] stations = new Station[0];
		private bool waiting = true;
		private bool buffering = false;
		private bool asleep = false;
		
		private string cacheDir = null;
		public string CacheDirectory {
			get {
				if (cacheDir == null) {
					cacheDir = Directory.GetCurrentDirectory() + "/cache";
					if (!Directory.Exists(CacheDirectory))
						Directory.CreateDirectory(CacheDirectory);
				}
				return cacheDir;
			}
		}

		public Song[] Playlist {
			get {
				return playlist.ToArray();
			}
		}

		public delegate void SongEventHandler(object sender, Song song);
		public delegate void StationEventHandler(object sender, Station station);
		public delegate void ProgressEventHandler(object sender, int position, int duration, double progress);

		public event ProgressEventHandler Progress;
		public event SongEventHandler PlayingSong;
		public event SongEventHandler SongPaused;
		public event SongEventHandler SongUnPaused;
		public event SongEventHandler SongRatingChanged;
		public event EventHandler Buffering;
		public event EventHandler InvalidLogin;
		public event StationEventHandler StationChanged;

		public Player() {
			api = new APIWrapper();
			gStreamer.Stopped += gStreamer_Stopped;
			gStreamer.Progress += gStreamer_Progress;
			clearCacheDirectory();
		}

		public Player(string username, string password, string stationID = null):this() {
			LogIn(username, password, stationID);
		}

		public void LogIn(string username, string password, string stationID = null){
			bool success;
			stations = api.UserLogin(username, password, out success);
			if (!success) {
				EventHandler invalidLoginHandler = InvalidLogin;
				if (invalidLoginHandler != null)
					invalidLoginHandler(this, null);
			}
			if (stations.Length == 0)
				return;
			bool stationIDStored = false;
			if (stationID != null) {
				for (int x = 0; x < stations.Length; x++) {
					Station thisStation = stations[x];
					if (thisStation.Token == stationID) {
						currentStation = x;
						stationIDStored = true;
						break;
					}
				}
			}
			if (!stationIDStored)
				PreferencesManager.SetPreference(Preferences.StationID, stations[currentStation].Token);
			StationEventHandler stationChangedHandler = StationChanged;
			if (stationChangedHandler != null)
				stationChangedHandler(this, stations[currentStation]);
			playNext();
		}

		public void PlayPause() {
			if (gStreamer.IsPlaying()) {
				gStreamer.Pause();
				SongEventHandler pauseHandler = SongPaused;
				if (pauseHandler != null)
					pauseHandler(this, playlist[playlistPosition]);
			} else if(!waiting) {
				gStreamer.Play();
				SongEventHandler unPauseHandler = SongUnPaused;
				if (unPauseHandler != null)
					unPauseHandler(this, playlist[playlistPosition]);
			}
		}

		public void Rewind() {
			gStreamer.Rewind();
		}

		public void NextSong() {
			//TODO: api call to sleep the current song - or maybe not...
			if(!waiting)
				playNext();
		}

		public void ThumbsUp() {
			Song currentSong = playlist[playlistPosition];
			if (!currentSong.ThumbsUp) {
				api.AddFeedback(currentSong.SongID, true);
				currentSong.ThumbsUp = true;
				currentSong.ThumbsDown = false;
				SongEventHandler ratingChangedHandler = SongRatingChanged;
				if (ratingChangedHandler != null)
					ratingChangedHandler(this, currentSong);
			}
		}

		public void ThumbsDown() {
			Song currentSong = playlist[playlistPosition];
			if (!currentSong.ThumbsDown) {
				api.AddFeedback(currentSong.SongID, false);
				currentSong.ThumbsUp = false;
				currentSong.ThumbsDown = true;
				SongEventHandler ratingChangedHandler = SongRatingChanged;
				if (ratingChangedHandler != null)
					ratingChangedHandler(this, currentSong);
			}
			playNext();
		}

		public void NextStation() {
			playlist = new List<Song>();
			playlistPosition = bufferPosition = -1;
			waiting = true;
			buffering = false;
			currentStation++;
			if (currentStation >= stations.Length)
				currentStation = 0;
			gStreamer.Stop();
			clearCacheDirectory();
			StationEventHandler stationChangedHandler = StationChanged;
			if (stationChangedHandler != null)
				stationChangedHandler(this, stations[currentStation]);
			playNext();
			PreferencesManager.SetPreference(Preferences.StationID, stations[currentStation].Token);
		}

		public void PreviousStation() {
			playlist = new List<Song>();
			playlistPosition = bufferPosition = -1;
			waiting = true;
			buffering = false;
			currentStation--;
			if (currentStation < 0)
				currentStation = stations.Length - 1;
			gStreamer.Stop();
			clearCacheDirectory();
			StationEventHandler stationChangedHandler = StationChanged;
			if (stationChangedHandler != null)
				stationChangedHandler(this, stations[currentStation]);
			playNext();
			PreferencesManager.SetPreference(Preferences.StationID, stations[currentStation].Token);
		}

		public void Sleep() {
			asleep = true;
			if(gStreamer.IsPlaying())
				gStreamer.Pause();
		}

		public void WakeUp() {
			asleep = false;
			if (gStreamer.State == Gst.State.Paused)
				gStreamer.Play();
			else
				playNext();
			if (bufferPosition < (playlist.Count - 1))
				ThreadPool.QueueUserWorkItem(o => bufferNext());
		}

		public Song GetNextSong() {
			return playlist.Count > (playlistPosition + 1) ? playlist[playlistPosition + 1] : null;
		}

		private void bufferNext() {
			if (asleep)
				return;
			buffering = true;
			bufferPosition++;
			Song song = playlist[bufferPosition];
			//Console.WriteLine("buffering " + song.SongName + " by " + song.ArtistName);
			if (!File.Exists(CacheDirectory + "/" + song.SongID + ".mp3")) {
				try {
					WebRequest req = WebRequest.Create(song.AudioURL);
					WebResponse response = req.GetResponse();
					Stream rawMp3Stream = response.GetResponseStream();
					FileStream fileStream = File.Open(CacheDirectory + "/" + song.SongID + ".mp3", FileMode.Create);
					rawMp3Stream.CopyTo(fileStream);
					fileStream.Close();
				} catch {
					bufferPosition--;
					if (!asleep)
						ThreadPool.QueueUserWorkItem(o => bufferNext());
					return;
				}
			}
			if (waiting && !asleep)
				playSong(song);
			if (bufferPosition < (playlist.Count - 1) && !asleep)
				ThreadPool.QueueUserWorkItem(o => bufferNext());
			else
				buffering = false;
		}

		private void playNext() {
			if (asleep)
				return;
			if(gStreamer.IsPlaying())
				gStreamer.Pause();
			//delete the previous song from the cache
			if (playlist.Count == 0 || (playlist.Count <= playlistPosition + 2 && !buffering))
				getNextSongs();
			playlistPosition++;
			if (bufferPosition < playlistPosition)
				waiting = true;
			else
				playSong(playlist[playlistPosition]);
			if (waiting) {
				EventHandler bufferHandler = Buffering;
				if (bufferHandler != null)
					bufferHandler(this, EventArgs.Empty);
			}
		}

		private void playSong(Song song){
			if (asleep)
				return;
			waiting = false;
			gStreamer.Play(CacheDirectory + "/" + song.SongID + ".mp3");
			if (playlistPosition > 0) {
				string previousSongPath = CacheDirectory + "/" + playlist[playlistPosition - 1].SongID + ".mp3";
				if (File.Exists(previousSongPath))
					File.Delete(previousSongPath);
			}
			SongEventHandler playingHandler = PlayingSong;
			if(playingHandler != null)
				playingHandler(this, song);
		}

		private void getNextSongs() {
			if (asleep)
				return;
			playlist.AddRange(api.GetPlaylist(stations[currentStation].Token));
			if (!buffering)
				ThreadPool.QueueUserWorkItem(o => bufferNext());
		}

		private void clearCacheDirectory() {
			DirectoryInfo cache = new DirectoryInfo(CacheDirectory);
			FileInfo[] files = cache.GetFiles();
			foreach (FileInfo file in files) {
				try {
					file.Delete();
				} catch {
					//just skip it
				}
			}
		}


		private void gStreamer_Stopped(object sender, EventArgs e) {
			playNext();
		}

		void gStreamer_Progress(object sender, int position, int duration, double progress) {
			ProgressEventHandler handler = Progress;
			if (handler != null)
				handler.Invoke(this, position, duration, progress);
		}
	}
}
