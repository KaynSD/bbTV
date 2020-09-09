using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	/// <summary>
	/// Unknown Play; show the test card during this play for technical difficulties
	/// </summary>
	public class UnknownPlay : BBAbstractPlay
	{
		public override void Parse(BBGameState gameState)
		{
			this.gameState = gameState;
		}

		/// Literally "match any text"
		public override Regex lastUpdateMatches => new Regex(@"/([A-Za-zÀ-ÖØ-öø-ÿÀ-ÖØ-öø-ÿ \.\!]+)/g");
	}
}