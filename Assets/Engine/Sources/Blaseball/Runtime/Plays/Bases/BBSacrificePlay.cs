using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Runner scores home as a result of a bunt or a sac fly
	/// 
	/// Annoyingly, you won't know who the sacrifice was. Look at the previous update's
	/// batter to see who that was
	/// lastUpdate "X scores on the sacrifice."
	/// </summary>

	public class BBSacrificePlay : BBAbstractPlayerPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+) +scores on the sacrifice(?: fly|)");

		Match recordedRegexMatch;
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);
		}

		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Scorer () {
			string playerName = recordedRegexMatch.Groups[0].Value;
			string playerTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;
			return GetPlayerByName(playerName, playerTeam);
		}

	}
}