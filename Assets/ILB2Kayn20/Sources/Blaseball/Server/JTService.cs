using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using blaseball.db;
using blaseball.file;
using blaseball.runtime;
using blaseball.ui;
using blaseball.vo;
using EvtSource;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;

namespace blaseball.service {

	public class JTService : IBlaseballResultsService
	{
		// IBL SSE Service; Codename Jessica Telephone
		// Valid for Season 4

		[Inject]	public IBlaseballDatabase Database;
		[Inject]	public IBlaseballFileLoader FileLoader;
		[Inject]	public ApplicationConfig ApplicationConfig;
		[Inject]	public IUILogger Logger;
		[Inject]	public GameRunner GameRunner;

		public JTService(){}

		public BlaseballDatabaseResult OnDatabaseCreated {get; set;}
		public BlaseballDatabaseResult OnDatabaseFailed {get; set;}
		public BlaseballDatabaseResult OnIncomingData {get; set;}

		EventSourceReader bbConnection;

		public BBGameStateDelegate OnGameUpdateRecieved { get; set; }

		public bool isConnected = false;

		public void Connect() {

			if(isConnected) return; // Already connected...

			if(bbConnection == null) {
				bbConnection = new EventSourceReader(new Uri(ApplicationConfig.BlaseballServiceSSEPath));
			}
			
			Logger.Log("JTService Connecting");
			Logger.Log("~~Ring Ring... #NeverLookFEEDBack");
			bbConnection.MessageReceived += OnMessage;
			bbConnection.Disconnected += OnDisconnect;
			bbConnection.Start();
		}

		private void OnDisconnect(object sender, DisconnectEventArgs e)
		{
			isConnected = false;
			bbConnection.Start();
		}

		private void OnMessage(object sender, EventSourceMessageEventArgs e)
		{
			isConnected = true;
			Parse(e.Message);
		}

		private void Parse(string message)
		{
			Logger.Log("Incoming Packet...");
			FileLoader.LogRawData("raw", message);
			JObject jsonBlob = JObject.Parse(message);
			
			string msg = (jsonBlob["value"]["games"].ToString());
			BBScheduleUpdate Update = JsonUtility.FromJson<BBScheduleUpdate>(msg);
			foreach(BBGameState state in Update.schedule) {
				OnGameUpdateRecieved?.Invoke(state);
				FileLoader.LogGame(state.id, JsonUtility.ToJson(state));
				GameRunner.AddGameUpdate(state);
			}

			BBTomorrowsGames Forecast = JsonUtility.FromJson<BBTomorrowsGames>(msg);
			foreach(BBGameState state in Forecast.tomorrowSchedule) {
				OnGameUpdateRecieved?.Invoke(state);
				GameRunner.AddGameUpdate(state);
			}

			OnIncomingData?.Invoke();
		}

		public void Disconnect()
		{
			bbConnection.MessageReceived -= OnMessage;
			bbConnection.Disconnected -= OnDisconnect;
			bbConnection.Dispose();
			
			isConnected = false;
			bbConnection = null;
		}

		public void BuildDatabase(string league, DatabaseConfigurationOptions options)
		{
			Thread DatabaseBuilder = new Thread(() => _BuildDatabase(league, options));
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

		private void _BuildDatabase(string league, DatabaseConfigurationOptions options)
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
				Logger.Log("League Information Downloaded");
				Database.SetLeague(JsonUtility.FromJson<BBLeague>(leagueInformation));
				Logger.Log("League Information Entered");
				Thread.Sleep(shortDelay);
				
			}
			// Create Subleague Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.SUBLEAGUES) == DatabaseConfigurationOptions.SUBLEAGUES) {
				foreach(string subleague in Database.GetLeague().subleagues) {
					success = UpdateSubleague(subleague);
					if(!success) {
						OnDatabaseFailed?.Invoke();
						return;
					}
					Thread.Sleep(shortDelay);
				}
			}

			// Division Information
			if(options == DatabaseConfigurationOptions.COMPLETE || (options & DatabaseConfigurationOptions.DIVISIONS) == DatabaseConfigurationOptions.DIVISIONS) {
				foreach(string subleagueID in Database.GetSubleagueIDs()) {
					BBSubleague subleague = Database.GetSubleague(subleagueID);
					foreach(string division in subleague.divisions) {
						success = UpdateDivision(division);
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
				foreach(string divisionID in Database.GetDivisionIDs()) {
					BBDivision division = Database.GetDivision(divisionID);
					foreach(string teamID in division.teams) {
						success = UpdateTeam(teamID);
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
				foreach(string teamID in Database.GetTeamIDs()) {
					BBTeam team = Database.GetTeam(teamID);
					
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
						success = UpdatePlayers(shortList.ToArray(), Database);
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
				Database.lastUpdated = Helper.GetUnixTime();
			}

			Logger.Log("Live Database Downloaded. Saving Local Datablase");
			Database.Save();
			FileLoader.SetupStreamingAssets();
			OnDatabaseCreated?.Invoke();


		}

		/// <summary>
		/// Get up to date information on a Subleague (Good / Evil)
		/// </summary>
		/// <param name="subleague">ID of the subleague to get information on</param>
		/// <returns>True if successful, False on a Failure</returns>
		public bool UpdateSubleague(string subleague) {
			string subleagueInformation = Download($"https://blaseball.com/database/subleague?id={subleague}");
		
			if(subleagueInformation == "") {
				return false;
			}
			BBSubleague SubleagueVO = JsonUtility.FromJson<BBSubleague>(subleagueInformation);
			Database.SetSubleague(SubleagueVO);
			
			Logger.Log($"Subleague: {SubleagueVO.name}");
			return true;
		}

		/// <summary>
		/// Get up to date information on a Division (eg, Chaotic Good)
		/// Will contain a name and a list of team IDs, in no particular order?
		/// </summary>
		/// <param name="subleague">ID of the division to get information on</param>
		/// <returns>True if successful, False on a Failure</returns>
		public bool UpdateDivision(string divisionID) {
			string divisionInformation = Download($"https://www.blaseball.com/database/division?id={divisionID}");
		
			if(divisionInformation == "") {
				return false;
			}
			BBDivision DivisionVO = JsonUtility.FromJson<BBDivision>(divisionInformation);
			Database.SetDivision(DivisionVO);
			
			Logger.Log($"Division: {DivisionVO.name}");
			return true;
		}
		
		/// <summary>
		/// Get up to date division information from the Blaseball Datablase
		/// </summary>
		/// <returns>true if successful, false on a failure</returns>
		public bool UpdateDivisions(IBlaseballDatabase Database) {
			string divisionInformation = Download("https://blaseball.com/database/allDivisions");
			if(divisionInformation == "") {
				return false;
			}
			BBDivisionArray divisions = JsonUtility.FromJson<BBDivisionArray>("{\"divisions\":" + divisionInformation + "}");
			foreach(BBDivision division in divisions.divisions) {
				Database.SetDivision(division);
				Logger.Log($"Division: {division.name}");
			}

			return true;
		}

		[Serializable]
		internal class BBDivisionArray {
			#pragma warning disable CS0649 // It is used. It's a partial json utilty class
			public BBDivision[] divisions;
			#pragma warning restore CS0649
		}
		

		/// <summary>
		/// Get up to date information on a team
		/// </summary>
		/// <param name="teamID">the ID of the team to get information on</param>
		/// <returns>true if successful, false on a failure</returns>
		public bool UpdateTeam(string teamID) {
			string teamInformation = Download($"https://blaseball.com/database/team?id={teamID}");
			if(teamInformation == "") {
				return false;
			}
			BBTeam team = JsonUtility.FromJson<BBTeam>(teamInformation);
			Database.SetTeam(team);
			Logger.Log($"Team: {team.fullName} {Helper.ToEmoji(team.emoji)}");// {char.ConvertFromUtf32(emoji)}");

			return true;
		}

		public bool UpdatePlayer(string playerID) {
			string playerInformation = Download($"https://blaseball.com/database/players?ids={playerID}");
			if(playerInformation == "") {
				return false;
			}
			BBPlayer player = JsonUtility.FromJson<BBPlayer>(playerInformation);
			Database.SetPlayer(player);
			Logger.Log($"Player: {player.name}");
			return true;
		}

		public bool UpdatePlayers(string[] playerIDs, IBlaseballDatabase Database ){
			string s = "";
			for(int i = 0; i < playerIDs.Length; i++) {
				s += playerIDs[i];
				if(i != playerIDs.Length - 1) s+=",";
			}
			string playerInformation = Download($"https://blaseball.com/database/players?ids={s}");
			
			if(playerInformation == "") {
				return false;
			}
			string loggedPlayers = "";
			BBPlayersArray players = JsonUtility.FromJson<BBPlayersArray>("{\"players\":" + playerInformation + "}");
			foreach(BBPlayer player in players.players) {
				Database.SetPlayer(player);
				loggedPlayers+=$"{player.name},";
			}
			Logger.Log($"Players: {loggedPlayers.Substring(0,loggedPlayers.Length-1)}");
			return true;
		}
		
		[Serializable]
		internal class BBPlayersArray {
			#pragma warning disable CS0649 // It is used. It's a partial json utilty class
			public BBPlayer[] players;
			#pragma warning restore CS0649
		}
		


	}
}
