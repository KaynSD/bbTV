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

	public class BBStrikePlay : BBAbstractPlay
	{
		public enum Strike {
			UNKNOWN,
			SWINGING,
			LOOKING
		}
		public override Regex lastUpdateMatches => new Regex(@"([sS]trike, )([A-z]+)\. ([0-9]+)-([0-9]+)");
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			Match match = lastUpdateMatches.Match(gameState.lastUpdate);
			switch(match.Groups[1].Value){
				case "swinging" : TypeOfStrike = Strike.SWINGING;
				break;
				case "looking" : TypeOfStrike = Strike.SWINGING;
				break;
				default : TypeOfStrike = Strike.UNKNOWN;
				break;
			}
		}

		public Strike TypeOfStrike; 

	}
}