using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// Start Game; play the intro animations!
	/// lastUpdate: "Play ball!"
	/// </summary>
	public class StartGamePlay : BBAbstractPlay
	{
		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		public override Regex lastUpdateMatches => new Regex(@"Play ball");
	}
}