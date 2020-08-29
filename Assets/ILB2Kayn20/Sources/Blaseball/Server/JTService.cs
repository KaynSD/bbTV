using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using blaseball.db;
using blaseball.file;
using blaseball.vo;
using EvtSource;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace blaseball.service {

	public delegate void BlaseballDatabaseResult();
	public class JTService : IBlaseballResultsService
	{
		// IBL SSE Service; Codename Jessica Telephone
		// Valid for Season 4
		public JTService(IFileLoader logger, IBlaseballDatabase database)
		{
			bbConnection = new EventSourceReader(new Uri("https://www.blaseball.com/events/streamData"));
			Logger = logger;
			Database = database;
		}

		public BlaseballDatabaseResult OnDatabaseCreated;
		public BlaseballDatabaseResult OnDatabaseFailed;

		EventSourceReader bbConnection;

		public BBGameStateDelegate OnGameUpdateRecieved { get; set; }
		public IFileLoader Logger { get; }
		public IBlaseballDatabase Database { get; }

		public void Connect() {
			
			
			Debug.Log("Ring Ring... #NeverLookFEEDBack");
			bbConnection.MessageReceived += OnMessage;
			bbConnection.Disconnected += OnDisconnect;
			bbConnection.Start();
		}

		private void OnDisconnect(object sender, DisconnectEventArgs e)
		{
			bbConnection.Start();
		}

		private void OnMessage(object sender, EventSourceMessageEventArgs e)
		{
			Parse(e.Message);
			//Debug.Log(e.Message);

			//StreamWriter writer = new StreamWriter("C:/Users/Keith Evans/Desktop/test.json");
			//writer.WriteLine(e.Message);
			//writer.Close();
		}

		private void Parse(string message)
		{
			JObject jsonBlob = JObject.Parse(message);
			
			string msg = (jsonBlob["value"]["games"].ToString());
			BBScheduleUpdate Update = JsonUtility.FromJson<BBScheduleUpdate>(msg);

			foreach(BBGameState state in Update.schedule) {
				OnGameUpdateRecieved?.Invoke(state);
				Logger.SaveLog(Database, state.id, JsonUtility.ToJson(state));
			}
		}

		public void Disconnect()
		{
			bbConnection.MessageReceived -= OnMessage;
			bbConnection.Disconnected -= OnDisconnect;
			bbConnection.Dispose();
		}

		public void BuildDatabase(string league, IBlaseballDatabase database, DatabaseConfigurationOptions options)
		{
			Thread DatabaseBuilder = new Thread(() => _BuildDatabase(league, database, options));
			DatabaseBuilder.Start();
		}

		private string Download(string uri) {
			string value = "";
			try {
				value = new WebClient().DownloadString(uri);
			} catch(WebException webEx) {
				Debug.LogError(webEx);
			}
			return value;
		}

		private void _BuildDatabase(string league, IBlaseballDatabase database, DatabaseConfigurationOptions options)
		{
			int shortDelay = 2000; //ms
			bool success = true;

			// Create League Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.LEAGUE) == DatabaseConfigurationOptions.LEAGUE) {
				if(league == "") {
					league = "d8545021-e9fc-48a3-af74-48685950a183";
				}
				string leagueInformation = Download($"https://blaseball.com/database/league?id={league}");
				if(leagueInformation == "") {
					OnDatabaseFailed?.Invoke();
					return;
				}
				Log("League Information Downloaded");
				database.SetLeague(JsonUtility.FromJson<BBLeague>(leagueInformation));
				Log("League Information Entered");
				Thread.Sleep(shortDelay);
				
			}
			// Create Subleague Information
			if(options == DatabaseConfigurationOptions.SUBLEAGUES || (options & DatabaseConfigurationOptions.SUBLEAGUES) == DatabaseConfigurationOptions.SUBLEAGUES) {
				foreach(string subleague in database.GetLeague().subleagues) {
					success = UpdateSubleague(subleague, database);
					if(!success) {
						OnDatabaseFailed?.Invoke();
						return;
					}
					Thread.Sleep(shortDelay);
				}
			}

			// Division Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.DIVISIONS) == DatabaseConfigurationOptions.DIVISIONS) {
				foreach(string subleagueID in database.GetSubleagueIDs()) {
					BBSubleague subleague = database.GetSubleague(subleagueID);
					foreach(string division in subleague.divisions) {
						success = UpdateDivision(division, database);
						if(!success) {
							OnDatabaseFailed?.Invoke();
							return;
						}
						Thread.Sleep(shortDelay);
					}

				}
			}
			

			// Teams Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.TEAMS) == DatabaseConfigurationOptions.TEAMS) {
				foreach(string divisionID in database.GetDivisionIDs()) {
					BBDivision division = database.GetDivision(divisionID);
					foreach(string teamID in division.teams) {
						success = UpdateTeam(teamID, database);
						if(!success) {
							OnDatabaseFailed?.Invoke();
							return;
						}
						Thread.Sleep(shortDelay);
					}
				}
			}

			// Players Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.PLAYERS) == DatabaseConfigurationOptions.PLAYERS) {
			List<string> players = new List<string>();
				foreach(string teamID in database.GetTeamIDs()) {
					BBTeam team = database.GetTeam(teamID);
					
					foreach(string playerID in team.lineup) {
						players.Add(playerID);
					}
					foreach(string playerID in team.rotation) {
						players.Add(playerID);
					}
				}

				List<string> shortList = new List<string>();
				while(players.Count > 0) {
					shortList.Add(players[0]);
					players.RemoveAt(0);

					if(shortList.Count >= 32 || players.Count == 0) {
						success = UpdatePlayers(shortList.ToArray(), database);
						if(!success) {
							OnDatabaseFailed?.Invoke();
							return;
						}
						Thread.Sleep(shortDelay);
						shortList = new List<string>();
					}
				}
			}

			if(options == DatabaseConfigurationOptions.COMPLETE) {
				database.lastUpdated = System.DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			}
			OnDatabaseCreated?.Invoke();


		}

		private void Log(string v)
		{
			Debug.Log($"{v}");
		}

		/// <summary>
		/// Get up to date information on a Subleague (Good / Evil)
		/// </summary>
		/// <param name="subleague">ID of the subleague to get information on</param>
		/// <returns>True if successful, False on a Failure</returns>
		public bool UpdateSubleague(string subleague, IBlaseballDatabase database) {
			string subleagueInformation = Download($"https://blaseball.com/database/subleague?id={subleague}");
		
			if(subleagueInformation == "") {
				return false;
			}
			BBSubleague SubleagueVO = JsonUtility.FromJson<BBSubleague>(subleagueInformation);
			database.SetSubleague(SubleagueVO);
			
			Log($"Subleague: {SubleagueVO.name}");
			return true;
		}

		/// <summary>
		/// Get up to date information on a Division (eg, Chaotic Good)
		/// Will contain a name and a list of team IDs, in no particular order?
		/// </summary>
		/// <param name="subleague">ID of the division to get information on</param>
		/// <returns>True if successful, False on a Failure</returns>
		public bool UpdateDivision(string divisionID, IBlaseballDatabase database) {
			string divisionInformation = Download($"https://www.blaseball.com/database/division?id={divisionID}");
		
			if(divisionInformation == "") {
				return false;
			}
			BBDivision DivisionVO = JsonUtility.FromJson<BBDivision>(divisionInformation);
			database.SetDivision(DivisionVO);
			
			Log($"Division: {DivisionVO.name}");
			return true;
		}
		
		/// <summary>
		/// Get up to date division information from the Blaseball Datablase
		/// </summary>
		/// <returns>true if successful, false on a failure</returns>
		public bool UpdateDivisions(IBlaseballDatabase database) {
			string divisionInformation = Download("https://blaseball.com/database/allDivisions");
			if(divisionInformation == "") {
				return false;
			}
			BBDivisionArray divisions = JsonUtility.FromJson<BBDivisionArray>("{\"divisions\":" + divisionInformation + "}");
			foreach(BBDivision division in divisions.divisions) {
				database.SetDivision(division);
				Log($"Division: {division.name}");
			}

			return true;
		}

		[Serializable]
		internal class BBDivisionArray {
			public BBDivision[] divisions;
		}
		

		/// <summary>
		/// Get up to date information on a team
		/// </summary>
		/// <param name="teamID">the ID of the team to get information on</param>
		/// <returns>true if successful, false on a failure</returns>
		public bool UpdateTeam(string teamID, IBlaseballDatabase database) {
			string teamInformation = Download($"https://blaseball.com/database/team?id={teamID}");
			if(teamInformation == "") {
				return false;
			}
			BBTeam team = JsonUtility.FromJson<BBTeam>(teamInformation);
			database.SetTeam(team);
			Log($"Team: {team.fullName}");

			return true;
		}

		public bool UpdatePlayer(string playerID, IBlaseballDatabase database) {
			string playerInformation = Download($"https://blaseball.com/database/players?ids={playerID}");
			if(playerInformation == "") {
				return false;
			}
			BBPlayer player = JsonUtility.FromJson<BBPlayer>(playerInformation);
			database.SetPlayer(player);
			Log($"Player: {player.name}");
			return true;
		}

		public bool UpdatePlayers(string[] playerIDs, IBlaseballDatabase database ){
			string s = "";
			for(int i = 0; i < playerIDs.Length; i++) {
				s += playerIDs[i];
				if(i != playerIDs.Length - 1) s+=",";
			}
			//Log($"Opening: https://blaseball.com/database/players?ids={s}");
			string playerInformation = Download($"https://blaseball.com/database/players?ids={s}");
			
			if(playerInformation == "") {
				return false;
			}
			string loggedPlayers = "";
			BBPlayersArray players = JsonUtility.FromJson<BBPlayersArray>("{\"players\":" + playerInformation + "}");
			foreach(BBPlayer player in players.players) {
				database.SetPlayer(player);
				loggedPlayers+=$"{player.name},";
			}
			Log($"Players: {loggedPlayers.Substring(0,loggedPlayers.Length-1)}");
			return true;
		}
		
		[Serializable]
		internal class BBPlayersArray {
			public BBPlayer[] players;
		}
		


	}
}
