using System;
using System.Collections;
using System.Collections.Generic;
using blaseball;
using blaseball.db;
using blaseball.runtime;
using blaseball.service;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class TitleScreen : MonoBehaviour
{
	[Inject] public IBlaseballDatabase database;
	[Inject]	public IBlaseballResultsService service;
	[Inject]	public GameRunner gameRunner;


	public GameObject GameButtonPrefab;
	public Transform currentGameList;
	public Transform futureGameList;
	public Transform historicalGameList;

	List<GameButtonBehaviour> currentGames;

	public List<GameObject> AllPanels;

	public TextMeshProUGUI lastDatabaseUpdateText;

	void Start() {
		database.Load();
		service.Connect();
		lastDatabaseUpdateText.SetText($"Last Cleaned: {Helper.GetLastUpdatedText(database.lastUpdated)}");
	
		currentGames = new List<GameButtonBehaviour>();

		HideAllPanels();
		RefreshDisplay();
		service.OnIncomingData += RefreshDisplay;

		//"d48564ae-6013-412c-8e2b-21fa73245b08";

		// Temporary Historical Game
		var gameFile = Resources.Load<TextAsset>("d48564ae-6013-412c-8e2b-21fa73245b08");
		BBReplay replay = JsonUtility.FromJson<BBReplay>(gameFile.text);
		for(int i = 0; i < replay.value.Length; i++) {
			gameRunner.AddGameUpdate(replay.value[i]);
		}

	}
	private void HideAllPanels()
	{
		foreach(GameObject panel in AllPanels) {
			panel.SetActive(false);
		}
	}


	public void ShowCurrentGames() {
		HideAllPanels();
		AllPanels[0].SetActive(true);
	}
	public void ShowForecastGames() {
		HideAllPanels();
		AllPanels[1].SetActive(true);
	}
	public void ShowHistoricalGames() {
		HideAllPanels();
		AllPanels[2].SetActive(true);
	}
	public void Exit() {
		//NoOp
	}

	public void ViewGame(GameButtonBehaviour button) {
		Debug.Log($"Loading Game: {button.GameID}");
		service.OnIncomingData -= RefreshDisplay;
		gameRunner.GameIDFocus = button.GameID;
		gameRunner.CurrentIndex = -1;
		SceneManager.LoadScene("ViewerScene");
	}

	private void RefreshDisplay() {
		currentGames.RemoveAll(x => x == null);

		foreach(KeyValuePair<string, BBGame> currentGame in gameRunner.Games) {
			string gameID = currentGame.Value.GameID;
			if(currentGames.Find(x => x.GameID == gameID)) continue;

			BBGameState state = currentGame.Value.current;

			GameButtonBehaviour.GameType gametype = GameButtonBehaviour.GameType.FORECAST;
			if(state.gameComplete) {
				gametype = GameButtonBehaviour.GameType.HISTORICAL;
			} else if(state.gameStart) {
				gametype = GameButtonBehaviour.GameType.CURRENT;
			}

			GameObject newGameButton = Instantiate(GameButtonPrefab);
			GameButtonBehaviour gbb = newGameButton.GetComponent<GameButtonBehaviour>();
			gbb.gameType = gametype;
			gbb.GameID = gameID;
			gbb.self.onClick.AddListener(() => ViewGame(gbb));

			switch(gametype) {
				case GameButtonBehaviour.GameType.CURRENT :
					newGameButton.transform.SetParent(currentGameList);
					break;
				case GameButtonBehaviour.GameType.FORECAST :
					newGameButton.transform.SetParent(futureGameList);
					break;
				default :
					newGameButton.transform.SetParent(historicalGameList);
					break;
					

			}

			currentGames.Add(gbb);

		}

	}

	public void DownloadDatabase() {
		service.BuildDatabase("", DatabaseConfigurationOptions.COMPLETE);
	}

	public void StartService() {
		service.Connect();
	}
}