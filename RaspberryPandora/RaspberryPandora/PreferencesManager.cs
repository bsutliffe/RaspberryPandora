using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RaspberryPandora {
	public static class PreferencesManager {
		private const string PrefsFilePath = "Prefs.txt";

		private static string[] prefs = null;

		public static string ReadPreference(Preferences pref){
			if (prefs == null) {
				if (File.Exists(PrefsFilePath))
					prefs = File.ReadAllLines(PrefsFilePath);
				else {
					prefs = new string[0];
					File.WriteAllLines(PrefsFilePath, prefs);
				}
			}
			return prefs.Length > (int)pref ? prefs[(int)pref] : null;
		}

		public static void SetPreference(Preferences pref, string value) {
			if (prefs.Length > (int)pref)
				prefs[(int)pref] = value;
			else {
				List<string> tempPrefs = prefs.ToList();
				while (tempPrefs.Count < (int)pref) {
					tempPrefs.Add(null);
				}
				tempPrefs.Add(value);
				prefs = tempPrefs.ToArray();
			}
			File.WriteAllLines(PrefsFilePath, prefs);
		}
	}

	public enum Preferences {
		Username = 0,
		Password = 1,
		StationID = 2
	}
}
