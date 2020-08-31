using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Player strikes out
	/// lastUpdate "X strikes out [type]." or "X struck out [type]".
	/// </summary>

	public class BBStrikeOutPlay : BBAbstractPlay
	{
		[Inject] public IBlaseballDatabase database; 
		public enum StrikeOut {
			UNKNOWN,
			SWINGING,
			LOOKING
		}
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+) (?:strikes out|struck out) ([A-Za-z]+)");

		Match recordedRegexMatch;
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);
			switch(recordedRegexMatch.Groups[1].Value){
				case "swinging" : TypeOfStrikeOut = StrikeOut.SWINGING;
				break;
				case "looking" : TypeOfStrikeOut = StrikeOut.SWINGING;
				break;
				default : TypeOfStrikeOut = StrikeOut.UNKNOWN;
				break;
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

		public StrikeOut TypeOfStrikeOut; 

	}
}