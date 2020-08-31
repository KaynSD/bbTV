using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Fielders Choice. A batter gets to base, at the expense of another player out
	/// It might result in a score; if Scorer is not null then someone scored
	/// lastUpdate "X reaches on fielder's choice. Y out at (second/third/fourth) base." Optionally followed by, " Z scores."
	/// </summary>

	public class BBFieldersChoice : BBAbstractPlay
	{
		/// <summary>
		/// Groups:
		/// 0 - Batter name, reaching
		/// 1 - Runner out
		/// 2 - Base out ( second, third, fourth)
		/// (Optional) 3 - Scoring Runner
		/// </summary>
		/// <returns></returns>
		public override Regex lastUpdateMatches => new Regex(@"([ A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+) reaches on fielder's choice\. ([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+) out at (first|second|third|home|fourth) base\.(?: ([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+)? scores\.?|)");
		public enum RunnerOutOn {
			UNKNOWN, FIRST, SECOND, THIRD, HOME
		}
		
		[Inject] public IBlaseballDatabase database; 

		public RunnerOutOn runnerGroundOutOn;

		Match recordedRegexMatch;
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);

			switch(recordedRegexMatch.Groups[2].Value) {
				case "first" : runnerGroundOutOn = RunnerOutOn.FIRST; break; // HOW?!
				case "second" : runnerGroundOutOn = RunnerOutOn.SECOND; break;
				case "third" : runnerGroundOutOn = RunnerOutOn.THIRD; break;
				case "home" :
				case "fourth" : runnerGroundOutOn = RunnerOutOn.HOME; break;
				default : runnerGroundOutOn = RunnerOutOn.UNKNOWN; break;
			}
		}

		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Batter () {
			string batterName = recordedRegexMatch.Groups[0].Value;
			string batterTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			BBTeam team = database.GetTeam(batterTeam);
			if(team == null) return null;
			
			foreach(string playerID in team.lineup){
				BBPlayer player = database.GetPlayer(playerID);
				if(player == null) continue;
				if(player.name == batterName) return player;
			}
			return null;
		}
		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer RunnerOut () {
			string batterName = recordedRegexMatch.Groups[1].Value;
			string batterTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			BBTeam team = database.GetTeam(batterTeam);
			if(team == null) return null;
			
			foreach(string playerID in team.lineup){
				BBPlayer player = database.GetPlayer(playerID);
				if(player == null) continue;
				if(player.name == batterName) return player;
			}
			return null;
		}

		/// <summary>
		/// Searches the database (specifically, just the batting team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Scorer () {
			if(recordedRegexMatch.Groups.Count <= 3) return null; 
			string batterName = recordedRegexMatch.Groups[3].Value;
			string batterTeam = gameState.topOfInning ? gameState.awayTeam : gameState.homeTeam;

			BBTeam team = database.GetTeam(batterTeam);
			if(team == null) return null;
			
			foreach(string playerID in team.lineup){
				BBPlayer player = database.GetPlayer(playerID);
				if(player == null) continue;
				if(player.name == batterName) return player;
			}
			return null;
		}
	}
}