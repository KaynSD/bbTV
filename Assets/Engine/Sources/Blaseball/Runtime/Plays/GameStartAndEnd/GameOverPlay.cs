using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// Game Over; Match is over. Show the winning team celebrating and the losing team commiserating
	///
	/// </summary>
	public class GameOverPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"Game over");

		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		/// <summary>
		/// The team with the highest score
		/// </summary>
		/// <returns>Winning Team ID</returns>
		public string WinningTeam () {
			return gameState.homeScore > gameState.awayScore ? gameState.homeTeam : gameState.awayTeam;
		}

		/// <summary>
		/// The team with the lowest score
		/// </summary>
		/// <returns>Losing Team ID</returns>
		public string LosingTeam () {
			return gameState.homeScore > gameState.awayScore ? gameState.awayTeam : gameState.homeTeam;
		}

		public int WinningScore () {
			return gameState.homeScore > gameState.awayScore ? gameState.homeScore : gameState.awayScore;
		}
		public int LosingScore () {
			return gameState.homeScore > gameState.awayScore ? gameState.awayScore : gameState.homeScore;
		}

		/// <summary>
		/// Return whether the result was an upset (in that the winning team had the lowest odds)
		/// </summary>
		/// <returns>True for Underdog victory, false for odds favourite</returns>
		public bool WasUpset() {
			if(WinningTeam() == gameState.homeTeam) {
				return gameState.homeOdds > gameState.awayOdds;
			} else {
				return gameState.awayOdds > gameState.homeOdds;
			}

		}
	}
}