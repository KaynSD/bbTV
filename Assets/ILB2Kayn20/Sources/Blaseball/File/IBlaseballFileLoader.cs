using blaseball.db;

namespace blaseball.file {
	/// <summary>
	/// The FileLoader interface handles loading and saving files to the user's hard
	/// drive.
	/// Dependency Injection handled by Zenject in the MainController
	/// </summary>
	public interface IBlaseballFileLoader
	{
		/// <summary>
		/// Log Data directly to the ApplicationConfig RootDirectory/raw/ folder
		/// </summary>
		/// <param name="path">Filename (appends .log) afterwards</param>
		/// <param name="data">Data to log to file</param>
		void LogRawData(string path, string data);

		/// <summary>
		/// Log, or append to, the game in the league's log folder.
		/// Filename will be (game-id).json
		/// </summary>
		/// <param name="id">the game id to log</param>
		/// <param name="json">a valid json object representing the data for this play / update</param>
		void LogGame(string id, string json);

		/// <summary>
		/// Setup the ApplicationConfig RootDirectory to have content mapped to the
		/// IBlaseballDatabase currently in memory. Different implementations may check
		/// pre-existing files or download content from Wikia or whatever later, but all
		/// will create directories, file structures and locations to store logs, raw data
		/// and access information from in future
		/// </summary>
		void SetupStreamingAssets();
	}
}