﻿using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.runtime;
using blaseball.runtime.events;
using blaseball.ui;
using blaseball.vo;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameViewer : MonoBehaviour
{
	[Inject] public GameRunner gameRunner; 
	[Inject] public IUILogger Logger;
	[Inject] public IBlaseballDatabase Database;
	[Inject] public BBPlaybook Playbook;

	protected BBGame game;
	
	void Start()
	{

		game = gameRunner.getFocusedGame();
		if(game == null){
			SceneManager.LoadScene("Title Scene");
			return;
		}

		game.OnUpdateReady += LogUpdate;
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
		Logger.Log($"UNHANDLED: {caseFail.gameState.lastUpdate}");
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
}