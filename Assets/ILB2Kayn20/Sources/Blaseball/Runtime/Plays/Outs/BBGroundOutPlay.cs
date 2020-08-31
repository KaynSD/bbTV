using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// A ground out occured;
	/// lastUpdate X hit a ground out to Y
	/// </summary>

	public class BBGroundOutPlay : BBAbstractPlay
	{

		[Inject]
		public IBlaseballDatabase database; 
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+)( hit a ground out to )([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+)");

		Match recordedRegexMatch;
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);
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
		/// Searches the database (specifically, just the fielding team) for a player
		/// with the matching name in the regex, and returns them, or null if not found
		/// </summary>
		/// <returns>The reference to the player, or null if failed</returns>
		public BBPlayer Fielder () {
			string fielderName = recordedRegexMatch.Groups[2].Value;
			string fielderTeam = gameState.topOfInning ? gameState.homeTeam : gameState.awayTeam;

			BBTeam team = database.GetTeam(fielderTeam);
			if(team == null) return null;

			foreach(string fielder in team.lineup){
				BBPlayer player = database.GetPlayer(fielder);
				if(player == null) continue;
				if(player.name == fielderName) return player;
			}
			return null;
		}
	}
}