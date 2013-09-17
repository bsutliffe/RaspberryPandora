namespace RaspberryPandora {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.picArt = new System.Windows.Forms.PictureBox();
			this.lblStation = new System.Windows.Forms.Label();
			this.lblTrack = new System.Windows.Forms.Label();
			this.pnlTrackInfo = new System.Windows.Forms.Panel();
			this.lblAlbum = new System.Windows.Forms.Label();
			this.lblArtist = new System.Windows.Forms.Label();
			this.lblNext = new System.Windows.Forms.Label();
			this.picThumbsUp = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picArt)).BeginInit();
			this.pnlTrackInfo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picThumbsUp)).BeginInit();
			this.SuspendLayout();
			// 
			// picArt
			// 
			this.picArt.InitialImage = null;
			this.picArt.Location = new System.Drawing.Point(3, 25);
			this.picArt.Name = "picArt";
			this.picArt.Size = new System.Drawing.Size(200, 200);
			this.picArt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picArt.TabIndex = 0;
			this.picArt.TabStop = false;
			// 
			// lblStation
			// 
			this.lblStation.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.lblStation.ForeColor = System.Drawing.Color.Navy;
			this.lblStation.Location = new System.Drawing.Point(3, 3);
			this.lblStation.Name = "lblStation";
			this.lblStation.Size = new System.Drawing.Size(446, 21);
			this.lblStation.TabIndex = 1;
			this.lblStation.Text = "Station Name";
			this.lblStation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblTrack
			// 
			this.lblTrack.AutoSize = true;
			this.lblTrack.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.lblTrack.ForeColor = System.Drawing.Color.Black;
			this.lblTrack.Location = new System.Drawing.Point(3, 3);
			this.lblTrack.MaximumSize = new System.Drawing.Size(250, 0);
			this.lblTrack.Name = "lblTrack";
			this.lblTrack.Size = new System.Drawing.Size(95, 17);
			this.lblTrack.TabIndex = 2;
			this.lblTrack.Text = "Track Name";
			// 
			// pnlTrackInfo
			// 
			this.pnlTrackInfo.AutoSize = true;
			this.pnlTrackInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlTrackInfo.Controls.Add(this.lblAlbum);
			this.pnlTrackInfo.Controls.Add(this.lblArtist);
			this.pnlTrackInfo.Controls.Add(this.lblTrack);
			this.pnlTrackInfo.Location = new System.Drawing.Point(207, 93);
			this.pnlTrackInfo.MaximumSize = new System.Drawing.Size(250, 0);
			this.pnlTrackInfo.Name = "pnlTrackInfo";
			this.pnlTrackInfo.Size = new System.Drawing.Size(101, 62);
			this.pnlTrackInfo.TabIndex = 3;
			// 
			// lblAlbum
			// 
			this.lblAlbum.AutoSize = true;
			this.lblAlbum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.lblAlbum.ForeColor = System.Drawing.Color.Black;
			this.lblAlbum.Location = new System.Drawing.Point(3, 47);
			this.lblAlbum.MaximumSize = new System.Drawing.Size(250, 0);
			this.lblAlbum.Name = "lblAlbum";
			this.lblAlbum.Size = new System.Drawing.Size(79, 15);
			this.lblAlbum.TabIndex = 4;
			this.lblAlbum.Text = "Album Name";
			// 
			// lblArtist
			// 
			this.lblArtist.AutoSize = true;
			this.lblArtist.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.lblArtist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.lblArtist.Location = new System.Drawing.Point(3, 25);
			this.lblArtist.MaximumSize = new System.Drawing.Size(250, 0);
			this.lblArtist.Name = "lblArtist";
			this.lblArtist.Size = new System.Drawing.Size(81, 17);
			this.lblArtist.TabIndex = 3;
			this.lblArtist.Text = "Artist Name";
			// 
			// lblNext
			// 
			this.lblNext.AutoEllipsis = true;
			this.lblNext.Location = new System.Drawing.Point(206, 209);
			this.lblNext.Name = "lblNext";
			this.lblNext.Size = new System.Drawing.Size(250, 13);
			this.lblNext.TabIndex = 4;
			this.lblNext.Text = "Next:";
			// 
			// picThumbsUp
			// 
			this.picThumbsUp.Image = global::RaspberryPandora.Properties.Resources.thumbs_up;
			this.picThumbsUp.Location = new System.Drawing.Point(207, 27);
			this.picThumbsUp.Name = "picThumbsUp";
			this.picThumbsUp.Size = new System.Drawing.Size(25, 30);
			this.picThumbsUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picThumbsUp.TabIndex = 5;
			this.picThumbsUp.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(464, 234);
			this.Controls.Add(this.picThumbsUp);
			this.Controls.Add(this.lblNext);
			this.Controls.Add(this.pnlTrackInfo);
			this.Controls.Add(this.lblStation);
			this.Controls.Add(this.picArt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "RaspberryPandora";
			((System.ComponentModel.ISupportInitialize)(this.picArt)).EndInit();
			this.pnlTrackInfo.ResumeLayout(false);
			this.pnlTrackInfo.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picThumbsUp)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picArt;
		private System.Windows.Forms.Label lblStation;
		private System.Windows.Forms.Label lblTrack;
		private System.Windows.Forms.Panel pnlTrackInfo;
		private System.Windows.Forms.Label lblAlbum;
		private System.Windows.Forms.Label lblArtist;
		private System.Windows.Forms.Label lblNext;
		private System.Windows.Forms.PictureBox picThumbsUp;

	}
}