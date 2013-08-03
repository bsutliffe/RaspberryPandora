using Gst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gst.GLib;
using Gst.CorePlugins;
using System.Threading;

namespace RaspberryPandora.GStreamer {
	public class GStreamerPlayer : IDisposable {
		private MainLoop loop;
		private Pipeline pipeline;
		private FileSrc filesrc;
		private Bin bin = null;
		private int playRetries = 0;

		public event EventHandler Stopped;

		public State State {
			get {
				State state;
				bin.GetState(out state, ulong.MaxValue);
				return state;
			}
		}

		public GStreamerPlayer() {
			Application.Init();
			loop = new MainLoop();
			pipeline = new Pipeline("audio-player");
			try {
				bin = (Bin)Parse.Launch("filesrc name=my_filesrc ! flump3dec ! alsasink");
			} catch {
				bin = (Bin)Parse.Launch("filesrc name=my_filesrc ! mad ! autoaudiosink");
			}
			if (bin == null)
				throw new Exception("Parse error.");
			filesrc = bin.GetByName("my_filesrc") as FileSrc;
			bin.Bus.AddWatch(new BusFunc(BusCb));
		}

		public void Play(string file = null) {
			if (file != null) {
				bin.SetState(State.Ready);
				filesrc.Location = file;
				playRetries = 0;
			}
			bin.SetState(State.Playing);
			if (!loop.IsRunning)
				ThreadPool.QueueUserWorkItem(o => loop.Run());
		}

		public void Pause() {
			bin.SetState(State.Paused);
		}

		public void Stop() {
			bin.SetState(State.Ready);
		}

		public void Rewind() {
			bin.SetState(State.Paused);
			bin.Seek(Gst.Format.Time, SeekFlags.Flush | SeekFlags.Accurate | SeekFlags.Skip, 0);
			bin.SetState(State.Playing);
		}

		public bool IsPlaying() {
			State state;
			bin.GetState(out state, ulong.MaxValue);
			if (state == State.Playing)
				return true;
			else
				return false;
		}

		private bool BusCb(Bus bus, Message message) {
			switch (message.Type) {
				case MessageType.Error:
					Enum err;
					string msg;
					message.ParseError(out err, out msg);
					if (err is Gst.ResourceError && (Gst.ResourceError)err == Gst.ResourceError.NotFound) {
						if (playRetries < 5) {
							Console.WriteLine("Stream not found.  Retrying...");
							System.Threading.Thread.Sleep(1000);
							playRetries++;
							Play();
							return true;
						} else {
							Console.WriteLine("Exceeded max play retires.");
						}
					}
					Console.WriteLine("Gstreamer error: " + err.ToString() + " " + msg);
					loop.Quit();
					//throw new Exception(msg);
					break;
				case MessageType.Eos:
					EventHandler handler = Stopped;
					if(handler != null)
						handler(this, EventArgs.Empty);
					break;
			}
			return true;
		}

		public void Dispose() {
			
		}
	}
}
