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
			history = new List<BBGameState>();
		}
		public string GameID;
		public bool isRunning;
		public List<BBGameState> history;
		public BBGameState current {get; protected set;}

		public BBGameStateDelegate OnUpdateReady;

		public void AddUpdate(BBGameState update) {
			if(update.id != GameID) return;
			
			history.Add(update);
			current = update;
			OnUpdateReady?.Invoke(update);
		}

		public BBGameState GetUpdate(int index)
		{
			if(index < 0) return history[history.Count - 1];
			if(history.Count < index) return history[index];
			return null;
		}

		public int HistoryLength => history.Count;

		private bool locked = false;
	}
}