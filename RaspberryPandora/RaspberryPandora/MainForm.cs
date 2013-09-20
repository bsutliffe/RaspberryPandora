using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RaspberryPandora {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
			Cursor cursor = new Cursor(Properties.Resources.Invisible.GetHicon());
			this.Cursor = cursor;
		}

		public void Reset(){
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(Reset));
			else {
				picArt.Image = null;
				picThumbsUp.Visible = false;
				lblStation.Text = lblTrack.Text = lblArtist.Text = lblAlbum.Text = lblNext.Text = "";
				lblCurTime.Text = lblDuration.Text = "00:00";
				progress.Value = progress.Minimum;
			}
		}

		public void NightMode(bool on = true) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { NightMode(on); }));
			this.BackColor = on ? Color.Gray : Color.White;
		}

		public void SetStation(string name) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { SetStation(name); }));
			else
				lblStation.Text = name;
		}

		public void SetAlbumArt(string location) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { SetAlbumArt(location); }));
			else {
				picArt.Image = null;
				picArt.ImageLocation = location;
			}
		}

		public void SetTrackInfo(string track, string artist, string album) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { SetTrackInfo(track, artist, album); }));
			else {
				lblTrack.Text = track;
				lblArtist.Text = artist;
				lblAlbum.Text = album;
				positionTrackInfoLabel();
			}
		}

		public void SetNextInfo(string track, string artist) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { SetNextInfo(track, artist); }));
			else
				lblNext.Text = "Next: " + artist + " - " + track;
		}

		public void ThumbsUp(bool value = true) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { ThumbsUp(value); }));
			else
				picThumbsUp.Visible = value;
		}

		public void SetProgress(int curTime, int duration, double? progress = null) {
			if (this.InvokeRequired)
				this.Invoke(new MethodInvoker(delegate { SetProgress(curTime, duration, progress); }));
			else {
				lblCurTime.Text = formatTime(curTime);
				lblDuration.Text = formatTime(duration);
				if (progress == null)
					progress = curTime == 0 || duration == 0 ? 0 : ((double)curTime / (double)duration);
				this.progress.Value = (int)(progress * this.progress.Maximum);
			}
		}

		private string formatTime(long time) {
			int min = (int)Math.Floor((decimal)time / (decimal)60);
			int sec = (int)time % 60;
			return min + ":" + (sec < 10 ? "0" : "") + sec;
		}

		private void positionTrackInfoLabel() {
			lblArtist.Top = lblTrack.Top + lblTrack.Height;
			lblAlbum.Top = lblArtist.Top + lblArtist.Height;
			pnlTrackInfo.Top = (picArt.Height / 2) - (pnlTrackInfo.Height / 2) + picArt.Top;
		}
	}
}
