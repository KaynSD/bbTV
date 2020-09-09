using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// Top of the inning; the away team is at bat
	/// lastUpdate: Top of X, Some team batting.
	///
	/// </summary>
	public class BBTopOfInningPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"Top of (\d+),");
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		/// <summary>
		/// Returns the team ID of the batting team (away, since it's the top)
		/// </summary>
		/// <returns>Away Team ID</returns>
		public string BattingTeam () {
			return gameState.awayTeam;
		}

		/// <summary>
		/// Returns the team ID of the batting team (home, since it's the top)
		/// </summary>
		/// <returns>Home Team ID</returns>
		public string FieldingTeam () {
			return gameState.homeTeam;
		}
	}
}