using System.Collections;
using blaseball.db;
using UnityEngine.UI;

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

		/// <summary>
		/// Get the custom path for the team logo, user specified
		/// Generally it will be {applicationConfig.RootDirectory}blaseball/{league.id}/team/{teamID}/logo.png
		/// </summary>
		/// <param name="teamID">The team ID</param>
		/// <returns>the expected filepath for the file</returns>
		string GetTeamTexturePath(string teamID);
		/// <summary>
		/// Get the 3D Model Unity3D Asset Bundle for the Team's Logo
		/// This GameObject exported should have a base gameobject name of "Logo" and have an Animate component
		/// attached
		/// Generally it will be located at {applicationConfig.RootDirectory}blaseball/{league.id}/team/{teamID}/logo.unity3d
		/// </summary>
		/// <param name="teamID">The team ID</param>
		/// <returns>The expected filepath for the logo</returns>
		string GetTeam3DLogoPath(string teamID);
		/// <summary>
		/// Get the 3D Model Unity3D Asset Bundle for this player's custom model
		/// The GameObject that loaded in should be named "Body" and have a fully rigged up CharacterCutsceneControl
		/// behaviour attached, including Animator, hand attachment points and facility to show name and team colours 
		/// 
		/// Generally it will be located at {applicationConfig.RootDirectory}blaseball/{league.id}/player/{playerID}/body.unity3d
		/// </summary>
		/// <param name="teamID">The player ID</param>
		/// <returns>The expected filepath for the custom model</returns>
		string GetPlayerCustomModelPath(string playerID);
		/// <summary>
		/// Get the file path for the player's custom Tlopps Baseball Card
		/// Generally it will be located at {applicationConfig.RootDirectory}blaseball/{league.id}/player/{playerID}/card.png
		/// </summary>
		/// <param name="teamID">The player ID</param>
		/// <returns>The expected filepath for the Tlopps Card</returns>
		string GetPlayerTloppsCard(string playerID);
	}
}