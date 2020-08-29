using System;
using System.Collections.Generic;
using blaseball.db;
using blaseball.vo;

namespace blaseball.runtime {
	[System.Serializable]
	public class BBGame
	{
		public BBGame(string gameID)
		{
			GameID = gameID;
			gameUpdates = new Queue<BBGameState>();
			history = new List<BBGameState>();
		}
		public string GameID;
		public bool isRunning;
		protected Queue<BBGameState> gameUpdates;
		public List<BBGameState> history;

		public BBGameStateDelegate OnUpdateReady;

		public void AddUpdate(BBGameState update) {
			if(update.id != GameID) return;

			gameUpdates.Enqueue(update);
			history.Add(update);
			if(gameUpdates.Count == 1 && !locked) {
				DispatchUpdate();
			}

		}

		public void DispatchUpdate()
		{
			if(gameUpdates.Count < 1) return;
			locked = true;
			BBGameState nextState = gameUpdates.Dequeue();

			OnUpdateReady?.Invoke(nextState);
			locked = false;
			
		}

		private bool locked = false;
	}
}