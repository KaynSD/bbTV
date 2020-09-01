using blaseball.db;
using blaseball.vo;

namespace blaseball.service {
	public delegate void BlaseballDatabaseResult();
	public interface IBlaseballResultsService {

		/// <summary>
		/// Construct a database from the Blaseball API
		/// The offline database is used to store as much as possible so that data isn't constantly
		/// polled from the website during games. Ideally, run this at a time when the site traffic is low (so while
		/// no games are running) to do your part to keep the site functional
		/// </summary>
		/// <param name="league">The league ID to get from blaseball.com; leave blank to use the default</param>
		/// <param name="options">Options; send in COMPLETE to scrape all the data, or mix and match different parameters to only update specific parts based on what knowledge the database already has</param>
		void BuildDatabase(string league, DatabaseConfigurationOptions options);
		/// <summary>
		/// Connect to the Blaseball SSE Service
		/// </summary>
		void Connect();
		/// <summary>
		/// Disconnect safely from the Blaseball SSE Service
		/// </summary>
		void Disconnect();
		/// <summary>
		/// Dispatched when an update is recieved from the Blaseball SSE service
		/// </summary>
		/// <value>Game Updated (relevant ID attached)</value>
		BBGameStateDelegate OnGameUpdateRecieved {get; set;}

		BlaseballDatabaseResult OnDatabaseCreated {get; set;}
		BlaseballDatabaseResult OnDatabaseFailed {get; set;}
		BlaseballDatabaseResult OnIncomingData {get; set;}
	}

	public enum DatabaseConfigurationOptions {
		NOTHING = 0,
		COMPLETE = 1,
		LEAGUE = 2,
		SUBLEAGUES = 4,
		DIVISIONS = 8,
		TEAMS = 16,
		PLAYERS = 32
	}
}