using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	/// <summary>
	/// Batter hits, and runs a Single, Double or Triple
	/// May also include scores. No outs.
	/// 
	/// If it includes scores then it's on the viewer to work out who just
	/// scored. This will mean cross referencing who was on the previous bases
	/// and who currently is not in the gameState.baseRunners
	/// 
	/// lastUpdate "X hits a [TYPE]!" or  "X hits a [TYPE]! [SCORE] scores"
	/// </summary>

	public class BBRunsPlay : BBAbstractPlay
	{
		public enum HitType {
			UNKNOWN,
			SINGLE,
			DOUBLE,
			TRIPLE
		}

		public override Regex lastUpdateMatches => new Regex(@"([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ]+) hits a (Single|Double|Triple)[\!\.] ?([0-9]+)?");

		public int scores = 0;
		public HitType hit = HitType.UNKNOWN;

		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
			Match recordedRegexMatch;
			recordedRegexMatch = lastUpdateMatches.Match(gameState.lastUpdate);
			switch(recordedRegexMatch.Groups[1].Value) {
				case "Single" : hit = HitType.SINGLE; break;
				case "Double" : hit = HitType.DOUBLE; break;
				case "Triple" : hit = HitType.TRIPLE; break;
			}

			// presence of a third group means score happened
			if(recordedRegexMatch.Groups.Count == 3) {
				scores = System.Convert.ToInt32(recordedRegexMatch.Groups[2]);
			}
		}

		/// <summary>
		/// The ID of the batter running; whoever is at the correct base!
		/// </summary>
		/// <returns>The batter ID or "" if not found</returns>
		public string BatterWalking(){

			// Find which base we're meant to be on
			int targetIndex = -1;
			switch(hit) {
				case HitType.SINGLE : targetIndex = 0; break;
				case HitType.DOUBLE : targetIndex = 1; break;
				case HitType.TRIPLE : targetIndex = 2; break;
			}
			//
			if(targetIndex == -1) return "";

			int batterIndex = -1;
			for(int i = 0; i < gameState.basesOccupied.Length; i++) {
				if(gameState.basesOccupied[0] == targetIndex) {
					batterIndex = i;
					break;
				}
			}

			if(batterIndex == -1) return "";
			return gameState.baseRunners[batterIndex];
		}

	}
}