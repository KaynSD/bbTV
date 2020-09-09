using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Pitcher throws a fourth ball and batter draws a walk
	/// lastUpdate "Strike, [type]. [balls]-[strikes]"
	/// </summary>

	public class BBDrawsAWalkPlay : BBAbstractPlay
	{
		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ ]+)draws a walk");

		/// <summary>
		/// The ID of the batter drawing a walk; whoever is at base 0
		/// </summary>
		/// <returns></returns>
		public string BatterWalking(){
			int batterIndex = -1;
			for(int i = 0; i < gameState.basesOccupied.Length; i++) {
				// 0 indexed. 0 is first base
				if(gameState.basesOccupied[0] == 0) {
					batterIndex = i;
					break;
				}
			}

			if(batterIndex == -1) return "";
			return gameState.baseRunners[batterIndex];
		}

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}
	}
}