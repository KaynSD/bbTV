using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Runner caught stealing a base
	/// lastUpdate "X gets caught stealing [base] base"
	/// </summary>

	public class BBCaughtStealingPlay : BBAbstractPlayerPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+) gets caught stealing ([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+)");
		
		public enum StolenBase {
			SECOND, THIRD, HOME, UNKNOWN
		}

		public StolenBase stolenBase;

		Match recordedRegexMatch;
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);

			switch(recordedRegexMatch.Groups[1].Value) {
				case "second" : stolenBase = StolenBase.SECOND; break;
				case "third" : stolenBase = StolenBase.THIRD; break;
				case "home" : 
				case "fourth": stolenBase = StolenBase.HOME; break;
				default: stolenBase = StolenBase.UNKNOWN; break;
			}
		}

		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Stealer () {
			string batterName = recordedRegexMatch.Groups[0].Value;
			string batterTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			return GetPlayerByName(batterName, batterTeam);
		}

	}
}