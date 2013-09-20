using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using PandoraSharp;
using System.Text.RegularExpressions;
using System.Collections;
using RaspberryPandora.Objects;
using fastJSON;

namespace RaspberryPandora {
	public class APIWrapper {

		private string authToken = null;
		private string partnerID = null;
		private string userID = null;
		private long syncTime = 0;
		private DateTime clientStartTime;

		public APIWrapper() {
			clientStartTime = DateTime.Now;
			partnerLogin();
		}

		public Station[] UserLogin(string username, string password, out bool success) {
			success = false;
			string body = @"{
				""loginType"": ""user"",
				""username"": """ + username + @""",
				""password"": """ + password + @""",
				""partnerAuthToken"": """ + authToken + @""",
				""includePandoraOneInfo"":false,
				""includeAdAttributes"":false,
				""includeSubscriptionExpiration"":false,
				""includeStationArtUrl"":true,
				""returnStationList"":true,
				""returnGenreStations"":false,
				""syncTime"":" + (syncTime + (DateTime.Now - clientStartTime).TotalMilliseconds) + @"
			}";
			Dictionary<string, object> response;
			string status = makeRequest("auth.userLogin", body, out response);
			List<Station> stationList = new List<Station>();
			if(status == "ok"){
				success = true;
				userID = response.ContainsKey("userId") ? response["userId"] as string : null;
				if (response.ContainsKey("userAuthToken"))
					authToken = response["userAuthToken"] as string;
				if (response.ContainsKey("stationListResult")) {
					List<object> stationArrayList = (List<object>)((Dictionary<string, object>)response["stationListResult"])["stations"];
					if (stationArrayList.Count > 0) {
						foreach (object stationObj in stationArrayList) {
							Dictionary<string, object> station = (Dictionary<string, object>)stationObj;
							if(station.ContainsKey("isQuickMix") && !(bool)station["isQuickMix"])
								stationList.Add(new Station {
									Token = station["stationToken"] as string,
									Name = station["stationName"] as string,
									ArtURL = station["artUrl"] as string
								});
						}
					}
				}
			}
			return stationList.ToArray();
		}

		public Song[] GetPlaylist(string stationID) {
			string body = @"{
				 ""userAuthToken"": """ + authToken + @""",
				 ""additionalAudioUrl"":  ""HTTP_128_MP3"",
				 ""syncTime"":" + (syncTime + (DateTime.Now - clientStartTime).TotalMilliseconds) + @",
				 ""stationToken"": """ + stationID + @"""
			}";
			Dictionary<string, object> response;
			string status = makeRequest("station.getPlaylist", body, out response);
			List<Song> songList = new List<Song>();
			if (status == "ok") {
				List<object> songObjList = response["items"] as List<object>;
				foreach (object songObj in songObjList) {
					Dictionary<string, object> thisSong = (Dictionary<string, object>)songObj;
					long? rating = thisSong["songRating"] as long?;
					songList.Add(new Song {
						SongID = (string)thisSong["trackToken"],
						SongName = (string)thisSong["songName"],
						ArtistName = (string)thisSong["artistName"],
						AlbumName = (string)thisSong["albumName"],
						AlbumArtURL = (string)thisSong["albumArtUrl"],
						AudioURL = (string)thisSong["additionalAudioUrl"],
						ThumbsUp = rating == 1
					});
				}
			}
			return songList.ToArray();
		}

		public bool AddFeedback(string songID, bool isPositive) {
			string body = @"{
				 ""trackToken"": """ + songID + @""",
				 ""isPositive"":" + (isPositive ? "true" : "false") + @"
			}";
			Dictionary<string, object> response;
			string status = makeRequest("station.addFeedback", body, out response);
			return status == "ok";
		}

		private bool partnerLogin() {
			string body = @"{
				""username"": ""android"",
				""password"": ""AC7IBG09A3DTSYM4R41UJWL07VLN8JI7"",
				""deviceModel"": ""android-generic"",
				""version"": ""5""
			}";
			Dictionary<string, object> response;
			string status = makeRequest("auth.partnerLogin", body, out response, false);
			if (status == "ok") {
				partnerID = response.ContainsKey("partnerId") ? response["partnerId"] as string : "42";
				authToken = response.ContainsKey("partnerAuthToken") ? response["partnerAuthToken"] as string : null;
				if (response.ContainsKey("syncTime")) {
					string rawSyncTime = response["syncTime"] as string;
					string decryptedSyncTime = PandoraCrypt.Decrypt(rawSyncTime);
					if (decryptedSyncTime != null &&  decryptedSyncTime.Length > 4)
						decryptedSyncTime = Regex.Replace(decryptedSyncTime.Substring(4), "[^0-9]", "");
					long.TryParse(decryptedSyncTime, out syncTime);
				}
				return true;
			}
			return false;
		}

		private string makeRequest(string method, string body, out Dictionary<string, object> responseObj, bool encrypt = true) {
			string uri = "http://tuner.pandora.com/services/json/";
			uri += "?method=" + method;
			if (authToken != null)
				uri += "&auth_token=" + HttpUtility.UrlEncode(authToken);
			if (partnerID != null)
				uri += "&partner_id=" + partnerID;
			if (userID != null)
				uri += "&user_id=" + userID;
			WebRequest req = HttpWebRequest.Create(uri);
			req.Method = "POST";
			if (encrypt)
				body = PandoraCrypt.Encrypt(body);
			byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
			req.ContentLength = bodyBytes.Length;
			req.ContentType = "application/json";
			Stream dataStream = req.GetRequestStream();
			dataStream.Write(bodyBytes, 0, bodyBytes.Length);
			dataStream.Close();
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();
			string status = null;
			if (response.StatusCode == HttpStatusCode.OK) {
				Stream responseStream = response.GetResponseStream();
				StreamReader reader = new StreamReader(responseStream);
				string stringResponse = reader.ReadToEnd();
				JsonParser parser = new JsonParser(stringResponse, false);
				Dictionary<string, object> outerResponse = (Dictionary<string, object>)parser.Decode();
				if (outerResponse.ContainsKey("stat") && (string)outerResponse["stat"] == "ok") {
					status = "ok";
					responseObj = (Dictionary<string, object>)outerResponse["result"];
				} else {
					status = outerResponse["stat"] as string;
					responseObj = null;
				}
				response.Close();
			} else {
				responseObj = null;
			}
			return status;
		}
	}
}
