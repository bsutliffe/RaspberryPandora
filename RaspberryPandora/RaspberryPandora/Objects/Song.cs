using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaspberryPandora.Objects {
	public class Song {
		public string SongID { get; set; }
		public string SongName { get; set; }
		public string ArtistName { get; set; }
		public string AlbumName { get; set; }
		public string AlbumArtURL { get; set; }
		public string AudioURL { get; set; }
		public bool ThumbsUp { get; set; }
		public bool ThumbsDown { get; set; }
	}
}
