using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// Bottom of the inning; the home team is at bat
	/// lastUpdate: Bottom of X, Some team batting.
	///
	/// </summary>
	public class BBBottomOfInningPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"Bottom of (\d+),");

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		/// <summary>
		/// Returns the team ID of the batting team (home, since it's the bottom)
		/// </summary>
		/// <returns></returns>
		public string BattingTeam () {
			return gameState.homeTeam;
		}

		/// <summary>
		/// Returns the team ID of the fielding team (away, since it's the bottom)
		/// </summary>
		/// <returns></returns>
		public string FieldingTeam () {
			return gameState.awayTeam;
		}
	}
}