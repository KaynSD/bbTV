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
	public class BBBatterAtPlatePlay : BBAbstractPlayerPlay
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
		public BBPlayer Batter () {
			// Top of inning means away team at bat
			return database.GetPlayer(gameState.topOfInning ?  gameState.awayBatter : gameState.homeBatter);
		}

		/// <summary>
		/// Returns the player ID of the player pitching
		/// </summary>
		/// <returns>Player ID of Pitcher</returns>
		public BBPlayer Pitcher () {
			return database.GetPlayer(gameState.topOfInning ?  gameState.homePitcher : gameState.awayPitcher);
		}
	}
}