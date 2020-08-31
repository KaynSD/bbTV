using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// New batter at plate
	/// lastUpdate: X batting for the Y.
	///
	/// </summary>
	public class BBBatterAtPlatePlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"(batting for the)");

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		/// <summary>
		/// Returns the player ID of the player at bat
		/// </summary>
		/// <returns>Player ID of Batter</returns>
		public string Batter () {
			// Top of inning means away team at bat
			return gameState.topOfInning ?  gameState.awayBatter : gameState.homeBatter;
		}

		/// <summary>
		/// Returns the player ID of the player pitching
		/// </summary>
		/// <returns>Player ID of Pitcher</returns>
		public string Pitcher () {
			// Top of inning means away team at bat
			return gameState.topOfInning ?  gameState.homePitcher : gameState.awayPitcher;
		}
	}
}