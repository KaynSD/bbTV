
using System;
using System.Collections.Generic;
using blaseball.vo;
using UnityEngine;

namespace blaseball.runtime {
	public class GameRunner {

		public string GameIDFocus = "";
		public Dictionary<string, BBGame> Games;
		public Dictionary<string, BBGame> TommorowsGames;

		public GameRunner()
		{
			Games = new Dictionary<string, BBGame>();
			TommorowsGames = new Dictionary<string, BBGame>();
		}

		internal BBGame getFocusedGame()
		{
			foreach(BBGame game in Games.Values) {
				if(game.GameID == GameIDFocus) return game;
			}
			foreach(BBGame game in TommorowsGames.Values) {
				if(game.GameID == GameIDFocus) return game;
			}
			return null;
		}

		public void AddGameUpdate(BBGameState newUpdateState) {
			string id = newUpdateState.id;

			if(TommorowsGames.ContainsKey(id)) {
				Games[id] = TommorowsGames[id];
				TommorowsGames.Remove(id);
			} else if(!Games.ContainsKey(id)) {
				Games[id] = new BBGame(id);	
			}

			TommorowsGames.Remove(id);

			BBGame gameToUpdate = Games[id];
			gameToUpdate.AddUpdate(newUpdateState);
		}

		public void CleanCurrentGames(){
			Games.Clear();
		}

		public void CleanTommorowsGames() {
			TommorowsGames.Clear();
		}
		public void AddTommorowsGame(BBGameState forecast) {
			string id = forecast.id;
			TommorowsGames[id] = new BBGame(id);	
			BBGame gameToUpdate = TommorowsGames[id];
			gameToUpdate.AddUpdate(forecast);
		}

		public void Clean(){
			TommorowsGames.Clear();
		}
	}
}