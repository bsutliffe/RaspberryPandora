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
		}

		public void Reset(){
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(Reset));
			else {
				picArt.Image = null;
				picThumbsUp.Visible = false;
				lblStation.Text = lblTrack.Text = lblArtist.Text = lblAlbum.Text = lblNext.Text = "";
			}
		}

		public void SetStation(string name) {
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(delegate { SetStation(name); }));
			else
				lblStation.Text = name;
		}

		public void SetAlbumArt(string location) {
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(delegate { SetAlbumArt(location); }));
			else {
				picArt.Image = null;
				picArt.ImageLocation = location;
			}
		}

		public void SetTrackInfo(string track, string artist, string album) {
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(delegate { SetTrackInfo(track, artist, album); }));
			else {
				lblTrack.Text = track;
				lblArtist.Text = artist;
				lblAlbum.Text = album;
				positionTrackInfoLabel();
			}
		}

		public void SetNextInfo(string track, string artist) {
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(delegate { SetNextInfo(track, artist); }));
			else
				lblNext.Text = "Next: " + artist + " - " + track;
		}

		public void ThumbsUp(bool value = true) {
			if (this.InvokeRequired == true)
				this.Invoke(new MethodInvoker(delegate { ThumbsUp(value); }));
			else
				picThumbsUp.Visible = value;
		}

		private void positionTrackInfoLabel() {
			lblArtist.Top = lblTrack.Top + lblTrack.Height;
			lblAlbum.Top = lblArtist.Top + lblArtist.Height;
			pnlTrackInfo.Top = (picArt.Height / 2) - (pnlTrackInfo.Height / 2) + picArt.Top;
		}
	}
}
