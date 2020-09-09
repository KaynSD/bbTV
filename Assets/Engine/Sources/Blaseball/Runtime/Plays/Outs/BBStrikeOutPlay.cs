using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Player strikes out
	/// lastUpdate "X strikes out [type]." or "X struck out [type]".
	/// </summary>

	public class BBStrikeOutPlay : BBAbstractPlayerPlay
	{
		public enum StrikeOut {
			UNKNOWN,
			SWINGING,
			LOOKING
		}
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+) (?:strikes out|struck out) ([A-Za-z]+)");

		Match recordedRegexMatch;
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);
			switch(recordedRegexMatch.Groups[2].Value){
				case "swinging" : TypeOfStrikeOut = StrikeOut.SWINGING;
				break;
				case "looking" : TypeOfStrikeOut = StrikeOut.LOOKING;
				break;
				default : TypeOfStrikeOut = StrikeOut.UNKNOWN;
				break;
			}
		}

		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Batter () {
			string playerName = recordedRegexMatch.Groups[1].Value;
			string playerTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			return GetPlayerByName(playerName, playerTeam);
		}

		/// <summary>
		/// Returns the player ID of the player pitching
		/// </summary>
		/// <returns>Player ID of Pitcher</returns>
		public BBPlayer Pitcher () {
			return database.GetPlayer(gameState.topOfInning ?  gameState.homePitcher : gameState.awayPitcher);
		}

		public StrikeOut TypeOfStrikeOut; 

	}
}