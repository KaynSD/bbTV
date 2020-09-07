using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using blaseball.db;
using blaseball.runtime;
using blaseball.vo;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

namespace blaseball.file {
	[System.Serializable]
	/// <summary>
	// IBL Unity Streaming Assets Fileloader; Codename Jessica Telephone
	/// </summary>
	public class JTFileLoader : IBlaseballFileLoader
	{
		[Inject]
		public IBlaseballDatabase database;
		[Inject]
		public ApplicationConfig applicationConfig;
		public JTFileLoader(){}


		public void LogGame(string id, string json)
		{
			BBLeague league = database.GetLeague();
			string logsPath = $"{applicationConfig.RootDirectory}/blaseball/{league.id}/logs";
			if(!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);

			string fileContents = json;
			string filePath = $"{logsPath}/{id}.json";


			if(File.Exists(filePath)) {
				string existing = File.ReadAllText(filePath);
				// Do not add to finalized games
				if(existing.Contains("\"finalized\":true")) return;
				
				fileContents = existing.Insert(existing.Length - 2, $",{fileContents}");



			} else {
				fileContents = "{\"value\": [" + fileContents + " ]}";
			}
			File.WriteAllText(filePath, fileContents);
		}

		public void LogRawData(string id, string content) {
			string rawPath = $"{applicationConfig.RootDirectory}/raw";
			if(!Directory.Exists(rawPath)) Directory.CreateDirectory(rawPath);

			string filePath = $"{rawPath}/{id}.log";

			File.WriteAllText(filePath, content);

		}

		public string GetTeamTexturePath(string teamID)
		{
			BBLeague league = database.GetLeague();
			return $"file://{applicationConfig.RootDirectory}blaseball/{league.id}/team/{teamID}/logo.png";
		}
		public string GetTeam3DLogoPath(string teamID)
		{
			BBLeague league = database.GetLeague();
			return $"{applicationConfig.RootDirectory}blaseball/{league.id}/team/{teamID}/logo.unity3d";
		}
		public string GetPlayerCustomModel(string playerID)
		{
			BBLeague league = database.GetLeague();
			return $"{applicationConfig.RootDirectory}blaseball/{league.id}/player/{playerID}/body.unity3d";
		}

		public void SetupStreamingAssets()
		{
			// Setup basics...
			if(!Directory.Exists(applicationConfig.RootDirectory)) Directory.CreateDirectory(applicationConfig.RootDirectory);

			string OSPath = $"{applicationConfig.RootDirectory}/blaseball";
			if(!Directory.Exists(OSPath)) Directory.CreateDirectory(OSPath);

			BBLeague league = database.GetLeague();

			string leaguePath = $"{OSPath}/{league.id}";
			string subleaguePath = $"{leaguePath}/subleague";
			string divisionPath = $"{leaguePath}/division";
			string teamPath = $"{leaguePath}/team";
			string playerPath = $"{leaguePath}/player";

			if(!Directory.Exists(leaguePath)) Directory.CreateDirectory(leaguePath);
			if(!Directory.Exists(subleaguePath)) Directory.CreateDirectory(subleaguePath);
			if(!Directory.Exists(divisionPath)) Directory.CreateDirectory(divisionPath);
			if(!Directory.Exists(teamPath)) Directory.CreateDirectory(teamPath);
			if(!Directory.Exists(playerPath)) Directory.CreateDirectory(playerPath);

			// Subleagues
			List<string> textFileContents = new List<string>();
			foreach(string subleague in database.GetSubleagueIDs()){
				string path = $"{subleaguePath}/{subleague}";
				if(!Directory.Exists(path)) Directory.CreateDirectory(path);

				BBSubleague subleagueVO = database.GetSubleague(subleague);
				textFileContents.Add($"{subleagueVO.name}\t{subleagueVO.id}");
			}
			CreateTextFile(textFileContents, subleaguePath);

			// Divisions
			textFileContents = new List<string>();
			foreach(string division in database.GetDivisionIDs()){
				string path = $"{divisionPath}/{division}";
				if(!Directory.Exists(path)) Directory.CreateDirectory(path);

				BBDivision divisionVO = database.GetDivision(division);
				textFileContents.Add($"{divisionVO.name}\t{divisionVO.id}");
			}
			CreateTextFile(textFileContents, divisionPath);

			// Teams
			textFileContents = new List<string>();
			foreach(string team in database.GetTeamIDs()){
				string path = $"{teamPath}/{team}";
				if(!Directory.Exists(path)) Directory.CreateDirectory(path);

				BBTeam teamVO = database.GetTeam(team);
				textFileContents.Add($"{teamVO.fullName}\t{teamVO.id}");
				
				string teamText = $"{teamVO.fullName}\n\"{teamVO.slogan}\"";
				File.WriteAllText($"{path}/info.txt", teamText);
			}
			CreateTextFile(textFileContents, teamPath);

			// Players
			textFileContents = new List<string>();
			foreach(string player in database.GetPlayerIDs()){
				string path = $"{playerPath}/{player}";
				if(!Directory.Exists(path)) Directory.CreateDirectory(path);

				BBPlayer playerVO = database.GetPlayer(player);
				textFileContents.Add($"{playerVO.name}\t{playerVO.id}");

				string playerText = $"{playerVO.name}\nBatting\t\t{StarRating(BBPlayer.BatterRating(playerVO))}\nPitching\t{StarRating(BBPlayer.PitcherRating(playerVO))}\n";
				playerText += $"Baserunning\t{StarRating(BBPlayer.BaserunningRating(playerVO))}\nDefense\t\t{StarRating(BBPlayer.DefenseRating(playerVO))}";
				File.WriteAllText($"{path}/info.txt", playerText);
			}
			CreateTextFile(textFileContents, playerPath);

			void CreateTextFile(List<string> content, string directory)
			{
				content.Sort();
				string fileContents = "";
				for(int i = 0; i < content.Count; i++) {
					fileContents += content[i] + "\n";
				}

				File.WriteAllText($"{directory}/index.tsv", fileContents);
			}

			string StarRating(float v)
			{
				string txt = "";
				int halfstars = Mathf.RoundToInt(v * 10);
				while(halfstars >= 2) {
					txt += "\u2605";
					halfstars -= 2;
				}
				if(halfstars == 1) {
					txt += "\u00BD";
				}
				return txt;
			}
		}

	}
}
