using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Fielder throws a ball
	/// lastUpdate BEGINS:"Ball. [balls]-[strikes]"
	/// </summary>

	public class BBBallPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"^Ball. [0-9]+-[0-9]+");

		
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

	}
}