using System.Collections.Generic;
using blaseball.vo;

namespace blaseball.db {
	[System.Serializable]
	/// <summary>
	// IBL SSE Database; Codename Jessica Telephone
	/// </summary>
	public class JTDatabase : IBlaseballDatabase
	{
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

		public double lastUpdated {get; set;}

		public bool Save(string filepath)
		{
			Serializer.Save<JTDatabase>(filepath, this);
			return true;
		}

		public static JTDatabase Load(string filepath) {
			return Serializer.Load<JTDatabase>(filepath);
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