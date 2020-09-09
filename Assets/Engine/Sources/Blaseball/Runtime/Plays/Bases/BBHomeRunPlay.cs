using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Batter hits a home run!
	/// lastUpdate "X hits a (2-run, 3-run, 4-run, solo) home run!"
	/// 
	/// If it's more than a solo then other runners will be scoring too
	/// Since the names and ids won't be in this play, check the PREVIOUS one
	/// for gameState.baseRunners; those are the additional runners scoring home
	/// right now
	/// 
	/// </summary>

	public class BBHomeRunPlay : BBAbstractPlayerPlay
	{

		/// <summary>
		/// Regex group captures;
		/// 0 - Batter Name
		/// Optional 1 - Number of Runs (1 is default)
		/// </summary>
		/// <returns></returns>
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+) hits a(?:(?: ([0-9]+)(?:-run )|(?: solo )|(?: )))home run!");
		public int scores = 1;
		Match recordedRegexMatch;

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);

			// presence of group 1 means we have multiple bases scoring
			// absence means it was a solo 
			if(recordedRegexMatch.Groups.Count == 2) {
				scores = System.Convert.ToInt32(recordedRegexMatch.Groups[1].Value);
			}
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

	}
}