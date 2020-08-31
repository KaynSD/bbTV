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

	public class BBFoulBallPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"Foul Ball. [0-9]+-[0-9]+");

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

	}
}