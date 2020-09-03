
using System;
using System.Collections.Generic;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime {
	public class GameRunner {

		public string GameIDFocus = "";
		public Dictionary<string, BBGame> Games;

		public int CurrentIndex = -1;

		public GameRunner()
		{
			Games = new Dictionary<string, BBGame>();
		}

		internal BBGame getFocusedGame()
		{
			foreach(BBGame game in Games.Values) {
				if(game.GameID == GameIDFocus) return game;
			}
			return null;
		}

		public void AddGameUpdate(BBGameState newUpdateState) {
			string id = newUpdateState.id;

			if(!Games.ContainsKey(id)) {
				Games[id] = new BBGame(id);	
			}

			BBGame gameToUpdate = Games[id];
			gameToUpdate.AddUpdate(newUpdateState);
		}

		public void CleanCurrentGames(){
			Games.Clear();
		}

	}
}