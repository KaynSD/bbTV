using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime.events {
	public abstract class BBAbstractPlay
	{
		/// <summary>
		/// Which index this is in the play order.
		/// </summary>
		public int playIndex = 0;

		/// <summary>
		/// Get the gameState attached to this play.
		/// This is normally assigned as part of the Parse method
		/// </summary>
		/// <value>The gameState attached</value>
		public BBGameState gameState {get; protected set;}

		/// <summary>
		/// The Regular Expression used to match the lastUpdated parameter
		/// of the Blaseball JSON update to work out, exactly, what just happened
		/// here.
		/// </summary>
		/// <value>A regular expression, or null (if not fully implemented!)</value>
		abstract public Regex lastUpdateMatches {get;}

		/// <summary>
		/// Parse the game state. Complicated plays will start working out, based
		/// on available resources to it and possible access to a database, what
		/// more complicated information needs to be gleaned for a proper visual
		/// display of what's happening
		/// 
		/// This method does NOT validate; it assumes that it has previously
		/// been matched to the lastUpdateMatches regular expression and thus can
		/// be correctly parsed. Match first, THEN parse.
		/// </summary>
		/// <param name="gameState">The game state to attach to this</param>
		abstract public void Parse(BBGameState gameState);

	}
}