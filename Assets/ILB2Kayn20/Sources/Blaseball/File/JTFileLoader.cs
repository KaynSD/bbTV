using System;
using System.Collections.Generic;
using System.IO;
using blaseball.db;
using blaseball.runtime;
using blaseball.vo;
using UnityEngine;

namespace blaseball.file {
	[System.Serializable]
	/// <summary>
	// IBL Unity Streaming Assets Fileloader; Codename Jessica Telephone
	/// </summary>
	public class JTFileLoader : IFileLoader
	{
		public JTFileLoader(string rootDirectory = "")
		{
			if(rootDirectory == "") rootDirectory = BBSettings.FILEPATH;
			Debug.Log($"RootDirectory = {rootDirectory}");
			RootDirectory = rootDirectory;
		}

		public string RootDirectory { get; }

		public void SaveLog(IBlaseballDatabase database, string id, string json)
		{
			BBLeague league = database.GetLeague();
			string logsPath = $"{RootDirectory}/blaseball/{league.id}/logs";

			string fileContents = json;
			string filePath = $"{logsPath}/{id}.json";


			if(File.Exists(filePath)) {
				string existing = File.ReadAllText(filePath);
				if(existing.Contains("Game over.")) return;
				
				fileContents = existing.Insert(existing.Length - 2, $",{fileContents}");



			} else {
				fileContents = "{\"value\": [" + fileContents + " ]}";
			}
			File.WriteAllText(filePath, fileContents);
		}

		public void SetupStreamingAssets(IBlaseballDatabase database)
		{
			// Setup basics...
			if(!Directory.Exists(RootDirectory)) Directory.CreateDirectory(RootDirectory);

			string OSPath = $"{RootDirectory}/blaseball";
			if(!Directory.Exists(OSPath)) Directory.CreateDirectory(OSPath);

			BBLeague league = database.GetLeague();

			string leaguePath = $"{OSPath}/{league.id}";
			string subleaguePath = $"{leaguePath}/subleague";
			string divisionPath = $"{leaguePath}/division";
			string teamPath = $"{leaguePath}/team";
			string playerPath = $"{leaguePath}/player";
			string logsPath = $"{leaguePath}/logs";

			if(!Directory.Exists(leaguePath)) Directory.CreateDirectory(leaguePath);
			if(!Directory.Exists(subleaguePath)) Directory.CreateDirectory(subleaguePath);
			if(!Directory.Exists(divisionPath)) Directory.CreateDirectory(divisionPath);
			if(!Directory.Exists(teamPath)) Directory.CreateDirectory(teamPath);
			if(!Directory.Exists(playerPath)) Directory.CreateDirectory(playerPath);
			if(!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);

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

		}

		private string StarRating(float v)
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

		private void CreateTextFile(List<string> textFileContents, string directory)
		{
			textFileContents.Sort();
			string fileContents = "";
			for(int i = 0; i < textFileContents.Count; i++) {
				fileContents += textFileContents[i] + "\n";
			}

			File.WriteAllText($"{directory}/index.tsv", fileContents);
		}

	}
}
