
using System.Collections.Generic;
using blaseball.vo;

namespace blaseball.runtime {
	[System.Serializable]
	public class GameRunner {
		protected Dictionary<string, BBGame> Games;

		public GameRunner()
		{
			Games = new Dictionary<string, BBGame>();
		}

		public void AddGameUpdate(BBGameState newUpdateState) {
			string id = newUpdateState.id;
			if(!Games.ContainsKey(id)) {
				Games[id] = new BBGame(id);	
			}

			BBGame gameToUpdate = Games[id];
			gameToUpdate.AddUpdate(newUpdateState);
		}
	}
}