using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// A flyout occured;
	/// lastUpdate X hit a flyout to Y
	/// </summary>

	public class BBFlyoutPlay : BBAbstractPlayerPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+)( hit a flyout to )([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+)");

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
		public BBPlayer Batter () {
			string playerName = recordedRegexMatch.Groups[0].Value;
			string playerTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			return GetPlayerByName(playerName, playerTeam);
		}
		/// <summary>
		/// Searches the database (specifically, just the fielding team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Fielder () {
			string playerName = recordedRegexMatch.Groups[2].Value;
			string playerTeam = gameState.topOfInning ? gameState.homeTeam : gameState.awayTeam;

			return GetPlayerByName(playerName, playerTeam);
		}
	}
}