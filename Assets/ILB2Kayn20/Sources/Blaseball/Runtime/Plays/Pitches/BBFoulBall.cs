using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Batter hits a foul. Counts as a strike if less than 3 strikes hit
	/// lastUpdate "Foul Ball. [balls]-[strikes]"
	/// </summary>

	public class BBFoulBallPlay : BBAbstractPlayerPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"Foul Ball. [0-9]+-[0-9]+");

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}
		
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