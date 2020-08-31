using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using blaseball.db;
using blaseball.runtime;
using blaseball.runtime.events;
using blaseball.ui;
using blaseball.vo;
using UnityEngine;
using Zenject;

public class GameDemonstration : MonoBehaviour
{
	[Inject] IUILogger Logger;
	[Inject] BBPlaybook Playbook;
	[Inject] IBlaseballDatabase Database;
	void Start() {
		Database.Load();

		// Sample match; Mills vs Firefighters, Season 4 Playoffs. Not the TDHAGOTET one sadly
		var gameFile = Resources.Load<TextAsset>("d48564ae-6013-412c-8e2b-21fa73245b08");
	
		BBReplay replay = JsonUtility.FromJson<BBReplay>(gameFile.text);

		BBGame game = new BBGame(replay.value[0].id);

		game.OnUpdateReady += LogUpdate;
		
		for(int i = 0; i < replay.value.Length; i++) {
			game.AddUpdate(replay.value[i]);
		}
	}

	private void LogUpdate(BBGameState gameState)
	{
		BBAbstractPlay play = Playbook.GetPlayFromState(gameState);

		switch(play) {
			case StartGamePlay case1: 
			HandleStartGame(case1); break;
			case BBTopOfInningPlay case2:
			HandleTopOfInnings(case2); break;
			case BBBottomOfInningPlay case3:
			HandleBottomOfInnings(case3); break;
			
			case BBBatterAtPlatePlay case4:
			BatterUp(case4); break;


			default:
			HandleTechnicalDifficulties(play);
			break;
		}
		//Logger.Log(gameState.lastUpdate);
	}

	private void HandleTechnicalDifficulties(BBAbstractPlay caseFail)
	{
		//Logger.Log($"Whoops we appear to be experiencing technical difficulties!\nbut what I can tell you is {caseFail.gameState.lastUpdate}");
	}

	private void HandleStartGame(StartGamePlay play)
	{
		BBTeam HomeTeam = Database.GetTeam(play.gameState.homeTeam);
		BBTeam AwayTeam = Database.GetTeam(play.gameState.awayTeam);
		Logger.Log($"Welcome splorts fans to this game here in {HomeTeam.location} for this titanic match between the {HomeTeam.fullName} and the {AwayTeam.fullName}");
	}

	private void ShowScore(BBGameState gameState)
	{
		BBTeam HomeTeam = Database.GetTeam(gameState.homeTeam);
		BBTeam AwayTeam = Database.GetTeam(gameState.awayTeam);
		Logger.Log($"{HomeTeam.fullName}, {gameState.homeScore} : {AwayTeam.fullName}, {gameState.awayScore}");
	}

	private void HandleTopOfInnings(BBTopOfInningPlay play)
	{
		BBTeam HomeTeam = Database.GetTeam(play.gameState.homeTeam);
		Logger.Log($"\nTop of the {1 + play.gameState.inning} and the {HomeTeam.nickname} take the field");
		if(play.gameState.inning > 0) ShowScore(play.gameState);
	}

	private void HandleBottomOfInnings(BBBottomOfInningPlay play)
	{
		BBTeam AwayTeam = Database.GetTeam(play.gameState.awayTeam);
		Logger.Log($"Bottom of the {1 + play.gameState.inning} and the {AwayTeam.nickname} take the field");
		if(play.gameState.inning > 0) ShowScore(play.gameState);
		if((play.gameState.inning + 1) % 7 == 0) Logger.Log("~~Take me out to the blall game~~");
	}

	private void BatterUp(BBBatterAtPlatePlay play) {
		BBPlayer batter = Database.GetPlayer(play.Batter());
		BBTeam team = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		Logger.Log($"Batting now for the {team.nickname} is {batter.name}, a {Math.Round(BBPlayer.BatterRating(batter) * 10) / 2} star batter");
	}

	class BBReplay {
		public BBGameState[] value;
	}
}

