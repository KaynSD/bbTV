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

		/// <summary>
		/// Get the Unix Timestamp
		/// </summary>
		/// <returns></returns>
		public static int GetUnixTime()
		{
			return (int)System.DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}
	}
}