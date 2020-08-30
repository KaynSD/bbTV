using System;
using System.IO;
using blaseball.db;
using blaseball.file;
using blaseball.runtime;
using blaseball.service;
using blaseball.vo;
using UnityEngine;

[System.Serializable]
public class Messager : MonoBehaviour
{
	public GameRunner gameRunner;
	
	public JTService service;
	public JTDatabase database;
	public JTFileLoader fileLoader;
	// Start is called before the first frame update
	void Start()
	{
		/*
		BBSettings.FILEPATH = "";
		gameRunner = new GameRunner();

		database = JTDatabase.Load(BBSettings.FILEPATH+"/blaseball/wafc.blbd");

		fileLoader = new JTFileLoader("");
		fileLoader.SetupStreamingAssets(database);


		service = new JTService(fileLoader, database);
		
		
		service.OnDatabaseCreated += OnFinish;
		service.OnDatabaseFailed += OnFailed;
		service.OnGameUpdateRecieved += OnUpdate;
		//service.BuildDatabase("", database, DatabaseConfigurationOptions.LEAGUE | DatabaseConfigurationOptions.TEAMS);
		service.Connect();

		*/
	}

	private void OnUpdate(BBGameState gameState)
	{
		gameRunner.AddGameUpdate(gameState);
	}

	private void OnFinish()
	{
		Debug.Log("Dictionary finished");
	}

	private void OnFailed()
	{
		Debug.Log("Failed to write dictionary");
	}

	void OnDestroy() {
		Debug.Log("WAFC! Dalé!");
		service.Disconnect();
	}
}
