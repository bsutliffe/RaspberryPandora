using RaspberryPandora.Objects;
using RaspberryPiDotNet;
using RaspberryPiDotNet.MicroLiquidCrystal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace RaspberryPandora {
	class Program {
		private static Player player;
		private static bool buttonsInitialized = false;
		private static LCDWrapper lcd;
		private static System.Timers.Timer shutdownTimer;

		static void Main(string[] args) {
			/*lcd = new LCDWrapper(
				rs: GPIOPins.V2_GPIO_24,
				enable: GPIOPins.V2_GPIO_17,
				d4: GPIOPins.V2_GPIO_18,
				d5: GPIOPins.V2_GPIO_27,
				d6: GPIOPins.V2_GPIO_22,
				d7: GPIOPins.V2_GPIO_23,
				columns: 40,
				rows: 2,
				displayMode: System.Environment.OSVersion.Platform == PlatformID.Win32NT ? DisplayMode.CONSOLE_ONLY : DisplayMode.BOTH
			);*/
			lcd = new LCDWrapper(
				rs: GPIOPins.V2_GPIO_24,
				enable: GPIOPins.V2_GPIO_17,
				d0: GPIOPins.V2_GPIO_09,
				d1: GPIOPins.V2_GPIO_10,
				d2: GPIOPins.V2_GPIO_11,
				d3: GPIOPins.V2_GPIO_14,
				d4: GPIOPins.V2_GPIO_18,
				d5: GPIOPins.V2_GPIO_27,
				d6: GPIOPins.V2_GPIO_22,
				d7: GPIOPins.V2_GPIO_23,
				columns: 40,
				rows: 2,
				displayMode: System.Environment.OSVersion.Platform == PlatformID.Win32NT ? DisplayMode.CONSOLE_ONLY : DisplayMode.BOTH
			);

			shutdownTimer = new System.Timers.Timer(20000);
			shutdownTimer.Elapsed += shutdownTimer_Elapsed;
			shutdownTimer.AutoReset = false;
			
			string username = PreferencesManager.ReadPreference(Preferences.Username);
			string password = PreferencesManager.ReadPreference(Preferences.Password);
			string stationID = PreferencesManager.ReadPreference(Preferences.StationID);
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
				Console.WriteLine("Enter your username: ");
				username = Console.ReadLine();
				Console.WriteLine("Enter your password: ");
				ConsoleKeyInfo key = Console.ReadKey(true);
				while (key.Key != ConsoleKey.Enter) {
					if(!char.IsControl(key.KeyChar))
						password += key.KeyChar;
					key = Console.ReadKey(true);
				}
				PreferencesManager.SetPreference(Preferences.Username, username);
				PreferencesManager.SetPreference(Preferences.Password, password);
			}

			lcd.Write("Connecting to Pandora...");
			player = new Player();

			player.PlayingSong += player_PlayingSong;
			player.SongPaused += player_SongPaused;
			player.SongUnPaused += player_SongUnPaused;
			player.Buffering += player_Buffering;
			player.SongRatingChanged += player_SongRatingChanged;
			player.StationChanged += player_StationChanged;

			lcd.Write("Logging in...");
			new Thread(o => player.LogIn(username, password, stationID)).Start();

			Console.ReadKey(true);
		}

		private static void initializeButtons() {
			buttonsInitialized = true;
			GPIO playPauseButton = GPIO.CreatePin(GPIOPins.V2_GPIO_04, GPIODirection.In);
			GPIO rewindButton = GPIO.CreatePin(GPIOPins.V2_GPIO_07, GPIODirection.In);
			GPIO nextSongButton = GPIO.CreatePin(GPIOPins.V2_GPIO_08, GPIODirection.In);
			//GPIO thumbsUpButton = GPIO.CreatePin(GPIOPins.V2_GPIO_09, GPIODirection.In);
			//GPIO thumbsDownButton = GPIO.CreatePin(GPIOPins.V2_GPIO_10, GPIODirection.In);
			//GPIO nextStationButton = GPIO.CreatePin(GPIOPins.V2_GPIO_11, GPIODirection.In);
			//GPIO prevStationButton = GPIO.CreatePin(GPIOPins.V2_GPIO_14, GPIODirection.In);
			GPIO powerToggle = GPIO.CreatePin(GPIOPins.V2_GPIO_25, GPIODirection.In);
			playPauseButton.AddChangeListener(pinChanged);
			rewindButton.AddChangeListener(pinChanged);
			nextSongButton.AddChangeListener(pinChanged);
			//thumbsUpButton.AddChangeListener(pinChanged);
			//thumbsDownButton.AddChangeListener(pinChanged);
			//nextStationButton.AddChangeListener(pinChanged);
			//prevStationButton.AddChangeListener(pinChanged);
			powerToggle.AddChangeListener(pinChanged);
		}


		private static void player_SongRatingChanged(object sender, Song song) {
			OutputNowPlaying(song);
		}

		private static void player_Buffering(object sender, EventArgs e) {
			lcd.Write("Buffering...");
		}

		private static void player_SongUnPaused(object sender, Objects.Song song) {
			//Console.WriteLine("UnPaused");	
		}

		private static void player_SongPaused(object sender, Objects.Song song) {
			//Console.WriteLine("Paused");
		}

		private static void player_PlayingSong(object sender, Objects.Song song) {
			OutputNowPlaying(song);
			if (!buttonsInitialized)
				initializeButtons();
		}

		private static void player_StationChanged(object sender, Objects.Station station) {
			lcd.Write(station.Name);
		}

		private static void OutputNowPlaying(Song song) {
			string[] lines = new string[]{
				(song.ThumbsUp ? "* " : "") + song.SongName,
				song.ArtistName + " / " + song.AlbumName
			};
			lcd.Write(lines);
		}


		private static void pinChanged(GPIO pin, bool value) {
			if ((int)pin.Pin == (int)GPIOPins.V2_GPIO_25) {
				//Power Toggle
				lcd.Power(value);
				if (value) {
					player.WakeUp();
					shutdownTimer.Stop();
					GPIOHandler.Restrict(GPIOPins.GPIO_NONE);
				} else {
					player.Sleep();
					shutdownTimer.Start();
					GPIOHandler.Restrict(GPIOPins.V2_GPIO_25);
				}
			} else if (value) {
				switch ((int)pin.Pin) {
					case (int)GPIOPins.V2_GPIO_04: // Play/Pause
						player.PlayPause();
						break;
					case (int)GPIOPins.V2_GPIO_07: // Rewind
						player.Rewind();
						break;
					case (int)GPIOPins.V2_GPIO_08: // Next Song
						player.NextSong();
						break;
					case (int)GPIOPins.V2_GPIO_09: // Thumbs Up
						player.ThumbsUp();
						break;
					case (int)GPIOPins.V2_GPIO_10: // Thumbs Down
						player.ThumbsDown();
						break;
					case (int)GPIOPins.V2_GPIO_11: // Next Station
						player.NextStation();
						break;
					case (int)GPIOPins.V2_GPIO_14: // Previous Station
						player.PreviousStation();
						break;
				}
			}
		}

		static void shutdownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			if (System.Environment.OSVersion.Platform == PlatformID.Unix) {
				Process p = new Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.FileName = "shutdown";
				p.StartInfo.Arguments = "now";
				p.Start();
			}
		}
	}
}
