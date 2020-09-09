using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.runtime.events {
	public abstract class BBAbstractPlayerPlay : BBAbstractPlay
	{
		protected IBlaseballDatabase database;  

		public void Setup(IBlaseballDatabase database) {
			this.database = database;
		}

		public BBPlayer GetPlayerByName(string playerName, string teamID) {

			BBTeam team = database.GetTeam(teamID);
			if(team != null) {
			
				foreach(string playerID in team.lineup){
					BBPlayer player = database.GetPlayer(playerID);
					if(player == null) continue;
					if(player.name == playerName) return player;
				}
			}

			return database.FindPlayerByName(playerName);
		}

	}
}