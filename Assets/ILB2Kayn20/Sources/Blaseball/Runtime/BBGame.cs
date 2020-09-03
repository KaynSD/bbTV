using System;
using System.Collections.Generic;
using blaseball.db;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime {
	[System.Serializable]
	public class BBGame
	{
		public BBGame(string gameID)
		{
			GameID = gameID;
			history = new List<BBGameState>();
		}
		public string GameID;
		public bool isRunning;
		public List<BBGameState> history;
		public BBGameState current {get; protected set;}

		public BBGameStateDelegate OnUpdateReady;

		/// <summary>
		/// Add an update to this game.
		/// If it is a legal update (as in, part of this game and not a duplicate) then OnUpdateReady
		/// will be dispatched with the update and the history list will be updated with this one at the end
		/// </summary>
		/// <param name="update">A potential update to add to this game</param>
		public void AddUpdate(BBGameState update) {

			// The update doesn't belong to this game. How did we get here?
			if(update.id != GameID) return;

			if(current != null) {
				// this update is a duplicate. Ignore!
				if(update.lastUpdate == current.lastUpdate) return;
			}
			
			history.Add(update);
			current = update;
			OnUpdateReady?.Invoke(update);
		}

		/// <summary>
		/// Get a specified update. Pass in a negative value or default -1 to
		/// get the most recent update, or pass in a non-zero value to get a specific
		/// update
		/// </summary>
		/// <param name="index">The specificed index of the update to get.
		/// A negative value will return the most recent update
		/// A positive or zero value will return the specified index (useful for rewinds or fast forwards) or null if it doesn't exist in this history</param>
		/// <returns>The update requested, or null</returns>
		public BBGameState GetUpdate(int index = -1)
		{
			if(index < 0) index = history.Count - 1;
			Debug.Log($"Getting index [{index} / {history.Count}]");
			if(history.Count > index) return history[index];
			return null;
		}


		/// <summary>
		/// The number of items in this game's history
		/// </summary>
		public int HistoryLength => history.Count;
	}
}