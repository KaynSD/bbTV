using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// A strike occured
	/// lastUpdate "Strike, [type]. [balls]-[strikes]"
	/// </summary>

	public class BBStrikePlay : BBAbstractPlayerPlay
	{
		public enum Strike {
			UNKNOWN,
			SWINGING,
			LOOKING
		}
		public override Regex lastUpdateMatches => new Regex(@"[sS]trike, ([A-Za-z]+)\. ([0-9]+)-([0-9]+)");
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			Match match = lastUpdateMatches.Match(gameState.lastUpdate);
			switch(match.Groups[1].Value){
				case "swinging" : TypeOfStrike = Strike.SWINGING;
				break;
				case "looking" : TypeOfStrike = Strike.LOOKING;
				break;
				default : TypeOfStrike = Strike.UNKNOWN;
				break;
			}
		}

		public Strike TypeOfStrike; 

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