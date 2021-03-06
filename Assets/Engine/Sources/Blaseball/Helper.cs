using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace blaseball {
	public static class Helper {

		/// <summary>
		/// Converts a hexadecimal representation of a Unicode codepoint into the literal
		/// character string, safe for display in Unity's text fields an TMP
		/// </summary>
		/// <param name="emojiStr">The hex value to pass through (eg, 0x1F525 for Fire)</param>
		/// <returns>The string literal for the unicode character (eg, 🔥)</returns>
		public static string ToEmoji(string emojiStr) {
			string raw = emojiStr;
			raw = raw.Remove(0,2);
			raw = raw.PadLeft(8, "0".ToCharArray()[0]);

			return char.ConvertFromUtf32(int.Parse(raw, NumberStyles.HexNumber));
		} 

		public static string GetLastUpdatedText(int databaseUpdated) {
			return new DateTime(1970, 1, 1).AddSeconds(databaseUpdated).ToString();
		}
		public static bool isDatabaseOld(int databaseUpdated) {
			TimeSpan age = DateTime.Now.Subtract(new DateTime(1970, 1, 1).AddSeconds(databaseUpdated));
			return age.TotalDays > 3;
		}

		/// <summary>
		/// Get the Unix Timestamp
		/// </summary>
		/// <returns></returns>
		public static int GetUnixTime()
		{
			return (int)System.DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public enum Weather {
			Void,
			Sunny, Overcast, Rainy, Sandstorm, Snowy, 
			Acidic, Eclipse, Glitter, Bloodwind, Peanuts,
			Birds, Feedback, Reverb, 
			

			Unknown
		}

		public static string GetStarRating(float rawPlayerValue) {
			int valueInHalfStars = Mathf.RoundToInt(rawPlayerValue * 10f);
			string r = "";
			for(int i = 0; i < valueInHalfStars / 2; i++) {
				r+="★";
			}
			if(valueInHalfStars % 2 == 1) r+= "½";
			return r;
		}
		public static Weather GetWeather(int weather)
		{
			if(weather >= 0 && weather <= 13) return (Weather)weather;
			return Weather.Unknown;
		}

		
	}
}