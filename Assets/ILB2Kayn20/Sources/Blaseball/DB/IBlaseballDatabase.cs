using System.Collections.Generic;
using blaseball.vo;

namespace blaseball.db {
	public interface IBlaseballDatabase
	{
		/// <summary>
		/// When the database was Last Cleaned and Updated
		/// </summary>
		/// <value>The last time a complete server scrape was completed, in Unix time</value>
		double lastUpdated {get; set;}

		/// <summary>
		/// Get the League inside the database connected to this endpoint
		/// </summary>
		/// <returns>The league datastructure, if present, or null</returns>
		BBLeague GetLeague();
		/// <summary>
		/// Set the League in this database
		/// This only sets the league data; you will have to change the Subleague and
		/// divisions (as well as teams and players afterwards)
		/// </summary>
		/// <param name="league">The league data object to pass in</param>
		void SetLeague(BBLeague league);

		/// <summary>
		/// A list of the subleague IDs stored within this database
		/// Use GetSubleague(string id) to get the specific subleague
		/// </summary>
		/// <returns>A list of the subleagues IDs in this database</returns>
		List<string> GetSubleagueIDs();
		/// <summary>
		/// Get the subleague (in IBL, Good or Evil) of the given ID. Subleagues will contain
		/// their name and Division ID, which can be obtained with GetDivision
		/// </summary>
		/// <param name="id">The id of the subleague to get</param>
		/// <returns>The subleague object, or null if not loaded</returns>
		BBSubleague GetSubleague(string id);
		/// <summary>
		/// Add or replace the Subleague in the database. The ID property of the subleague
		/// will be used as a unique identifier
		/// </summary>
		/// <param name="subleague">The subleague object</param>
		void SetSubleague(BBSubleague subleague);

		/// <summary>
		/// A list of division ids. These will be titled appropriately to their subleague
		/// You can then use GetDivision(id) to get the required division
		/// </summary>
		/// <returns>A list of division IDs present in this database</returns>
		List<string> GetDivisionIDs();
		/// <summary>
		/// Get the division of the specified ID. Divisions contain a name and a list of team IDs
		/// </summary>
		/// <param name="id">the division ID to get information for</param>
		/// <returns>the division object, or null if not present in the DB</returns>
		BBDivision GetDivision(string id);
		/// <summary>
		/// Add or replace the Division in the database. The ID property of the subleague
		/// will be used as a unique identifier
		/// </summary>
		/// <param name="division">The division object</param>
		void SetDivision(BBDivision division);

		/// <summary>
		/// A list of all the team ids in the database; should be around 20. 
		/// You can then use GetTeam(id) to get the required team
		/// </summary>
		/// <returns>A list of team IDs present in this database</returns>
		List<string> GetTeamIDs();
		/// <summary>
		/// Get the team of the specified ID. Team information contains four list of players
		/// as well as names, city location, colours, slogan, and other vital statistics
		/// </summary>
		/// <param name="id">the team ID to get information for</param>
		/// <returns>the team object, or null if not present in the DB</returns>
		BBTeam GetTeam(string id);
		/// <summary>
		/// Add or replace the team in the database. The ID property of the team
		/// will be used as a unique identifier
		/// </summary>
		/// <param name="team">The team object</param>
		void SetTeam(BBTeam team);

		/// <summary>
		/// A list of ALL the players in the database. 
		/// You can then use GetPlayer(id) to get the required player
		/// </summary>
		/// <returns>A list of player IDs present in this database</returns>
		List<string> GetPlayerIDs();
		/// <summary>
		/// Get the player of the specified ID. Player information is varied and complicated
		/// </summary>
		/// <param name="id">the player ID to get information for</param>
		/// <returns>the player object, or null if not present in the DB</returns>
		BBPlayer GetPlayer(string id);
		/// <summary>
		/// Add or replace the player in the database. The ID property of the player
		/// will be used as a unique identifier
		/// </summary>
		/// <param name="player">The player object</param>
		void SetPlayer(BBPlayer player);

		/// <summary>
		/// Save this database to file
		/// </summary>
		/// <param name="filepath">The absolute filepath to save</param>
		/// <returns>True if saved, false otherwise</returns>
		bool Save(string filepath);

	}
}