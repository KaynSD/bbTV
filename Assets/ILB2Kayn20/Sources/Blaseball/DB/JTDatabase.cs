using System;
using System.Collections.Generic;
using blaseball.ui;
using blaseball.vo;
using UnityEngine;
using Zenject;

namespace blaseball.db {
	[System.Serializable]
	/// <summary>
	// IBL SSE Database; Codename Jessica Telephone
	/// </summary>
	public class JTDatabase : IBlaseballDatabase
	{
		[Inject] [NonSerialized] public ApplicationConfig applicationConfig;
		[Inject] [NonSerialized] public IUILogger Logger;
		public JTDatabase()
		{
			League = new BBLeague();
			Subleagues = new Dictionary<string, BBSubleague>();
			Divisions = new Dictionary<string, BBDivision>();
			Teams = new Dictionary<string, BBTeam>();
			Players = new Dictionary<string, BBPlayer>();
		}

		protected BBLeague League;

		protected Dictionary<string, BBSubleague> Subleagues;

		protected Dictionary<string, BBDivision> Divisions;

		protected Dictionary<string, BBTeam> Teams;

		protected Dictionary<string, BBPlayer> Players;

		public int lastUpdated {get; set;}

		public bool Save()
		{
			Serializer.Save<JTDatabase>(applicationConfig.DatabaseLocation, this);
			Logger.Log("Datablase Saved");
			return true;
		}

		public bool Load() {
			JTDatabase database = Serializer.Load<JTDatabase>(applicationConfig.DatabaseLocation);


			if(database == null) {
				Logger.Log($"Database failed to load {applicationConfig.DatabaseLocation}. Creating new");
				Save();
				return false;
			}
			
			League = database.League;
			Subleagues = database.Subleagues;
			Divisions = database.Divisions;
			Teams = database.Teams;
			Players = database.Players;
			lastUpdated = database.lastUpdated;

			Logger.Log($"Datablase loaded, League is: {GetLeague().name} ({GetLeague().id})");
			Logger.Log($"Datablase Last Cleaned: {Helper.GetLastUpdatedText(database.lastUpdated)}");
			return true;
		}

		public BBLeague GetLeague() => League;
		public void SetLeague(BBLeague league){ 
			League = league;
		}

		public List<string> GetSubleagueIDs() => new List<string>(Subleagues.Keys);

		public BBSubleague GetSubleague(string id)
		{
			if(Subleagues.ContainsKey(id)) return Subleagues[id];
			return null;
		}

		public void SetSubleague(BBSubleague subleague) { 
			Subleagues[subleague.id] = subleague;
		}

		public List<string> GetDivisionIDs() => new List<string>(Divisions.Keys);

		public BBDivision GetDivision(string id)
		{
			if(Divisions.ContainsKey(id)) return Divisions[id];
			return null;
		}

		public void SetDivision(BBDivision division)
		{
			Divisions[division.id] = division;
		}

		public List<string> GetTeamIDs() => new List<string>(Teams.Keys);

		public BBTeam GetTeam(string id)
		{
			if(Teams.ContainsKey(id)) return Teams[id];
			return null;
		}

		public void SetTeam(BBTeam team)
		{
			Teams[team.id] = team;
		}

		public List<string> GetPlayerIDs() => new List<string>(Players.Keys);

		public BBPlayer GetPlayer(string id)
		{
			if(Players.ContainsKey(id)) return Players[id];
			return null;
		}

		public void SetPlayer(BBPlayer player)
		{
			Players[player.id] = player;
		}
	}
}