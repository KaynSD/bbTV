using System;
using System.Collections.Generic;
using System.IO;
using blaseball;
using blaseball.db;
using blaseball.file;
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
	[Inject]	public IBlaseballFileLoader loader;
	[Inject]	public ApplicationConfig config;
	[Inject]	public BBAnnouncements announcements;
	[Inject]	public GameRunner gameRunner;

	[Header("Everything")]
	public TextMeshProUGUI SplashCredits;

	[Header("Splash")]
	[SerializeField] protected RectTransform SplashRootElement;
	[SerializeField] protected TextMeshProUGUI SplashTagline;
	[SerializeField] protected Button SplashButton;

	[Header("Main Menu")]
	[SerializeField] protected RectTransform LeftMenuRootElement;
	[SerializeField] protected RectTransform SettingMenuRootElement;
	[SerializeField] protected Button CurrentGamesButton;
	[SerializeField] protected Button FutureGamesButton;
	[SerializeField] protected TextMeshProUGUI DatablaseUpdateText;
	[SerializeField] protected Button HistoricalGamesButton;
	[SerializeField] protected Button UpdateLocalDatablaseButton;
	[SerializeField] protected Button OpenLocalFolderButton;
	[SerializeField] protected Button GotoBlaseballButton;
	[SerializeField] protected Button GotoRepositoryButton;

	[Header("Current Games")]
	[SerializeField] RectTransform CurrentGamesRootElement;
	[SerializeField] protected Transform currentGameList;

	[Header("Future Games")]
	[SerializeField] RectTransform FutureGamesRootElement;
	[SerializeField] protected Transform futureGameList;
	[Header("Historical Games")]
	[SerializeField] RectTransform HistoricalGamesRootElement;
	[SerializeField] protected Transform historicalGameList;

	[Header("Other")]

	public GameObject GameButtonPrefab;
	List<GameButtonBehaviour> currentGames;
	private bool updateMessageTexts;

	void Start() {
		if(config.titleScreenSettings.needsToStartConnection) {
			database.Load();
			service.Connect();
		}

		service.OnDatabaseCreated += RefreshDisplay;
		service.OnIncomingData += RefreshDisplay;

		if(config.titleScreenSettings.bypassSplashScreen) {
			ShowMainMenu();
		} else {
			ShowSplashScreen();
		}
		
	
		currentGames = new List<GameButtonBehaviour>();

		//HideAllPanels();
		//RefreshDisplay();

		//"d48564ae-6013-412c-8e2b-21fa73245b08";

		// Temporary Historical Game
		/*
		try {
			var gameFile = Resources.Load<TextAsset>("d48564ae-6013-412c-8e2b-21fa73245b08");
			BBReplay replay = JsonUtility.FromJson<BBReplay>(gameFile.text);
			for(int i = 0; i < replay.value.Length; i++) {
				gameRunner.AddGameUpdate(replay.value[i]);
			}
		} catch (Exception c) {
			Debug.Log(c);
		}
		*/

	}

	void Update() {
		if(updateMessageTexts) {
			updateMessageTexts = false;
			UpdateDatabaseMessageTexts();
		}
	}

	public void DownloadDatabase() {
		UpdateLocalDatablaseButton.enabled = false;
		UpdateLocalDatablaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"updating...\n<size=50%>Downloading Updates...</size>");

		service.BuildDatabase("", DatabaseConfigurationOptions.COMPLETE);
	}

	public void StartService() {
		service.Connect();
	}

	void OnDestroy() {
		service.OnDatabaseCreated -= RefreshDisplay;
		service.OnIncomingData -= RefreshDisplay;
	}

	protected void Cleanup() {
		SplashRootElement.gameObject.SetActive(false);
		LeftMenuRootElement.gameObject.SetActive(false);
		SettingMenuRootElement.gameObject.SetActive(false);
		CurrentGamesRootElement.gameObject.SetActive(false);
		FutureGamesRootElement.gameObject.SetActive(false);
		HistoricalGamesRootElement.gameObject.SetActive(false);
	}

	public void ShowMainMenu () {
		Cleanup();
		LeftMenuRootElement.gameObject.SetActive(true);
		SettingMenuRootElement.gameObject.SetActive(true);
		updateMessageTexts = true;
	}
	
	public void UpdateDatabaseMessageTexts() {
		updateMessageTexts = false;
		UpdateLocalDatablaseButton.enabled = true;
		bool isUptoDate = Helper.isDatabaseOld(database.lastUpdated);
		
		if(isUptoDate) {
			DatablaseUpdateText.text = Constants.REMINDER_UPDATE_DATABLASE;
			UpdateLocalDatablaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"update local datablase\n<size=50%><color=red>Last Updated: {Helper.GetLastUpdatedText(database.lastUpdated)}</color></size>");
		} else {
			DatablaseUpdateText.text ="";
			UpdateLocalDatablaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"update local datablase\n<size=50%>Last Updated: {Helper.GetLastUpdatedText(database.lastUpdated)}</size>");
		}
	}

	public void OpenLocalFolder() {
		string itemPath = Path.Combine(config.RootDirectory, "blaseball");
		//EditorUtility.RevealInFinder(itemPath);
	}
	public void OpenBlaseballWebsite() => Application.OpenURL(Constants.URL_BLASEBALL);
	public void OpenBBTVRepo() => Application.OpenURL(Constants.URL_REPO);

	protected void ShowSplashScreen () {
		Cleanup();
		SplashRootElement.gameObject.SetActive(true);
		// Setup Splash Screen
		SplashTagline.text = announcements.GetAnnouncement(true);
		SplashCredits.text = $"V{config.VersionNumber} - {config.VersionName}\n{Constants.CREDITS}";
	}
	public void EndSplashScreen() {
		config.titleScreenSettings.bypassSplashScreen = true;
		ShowMainMenu();
	}

	public void ShowCurrentGames() {
		Cleanup();
		CurrentGamesRootElement.gameObject.SetActive(true);
	}
	public void ShowForecastGames() {
		Cleanup();
		FutureGamesRootElement.gameObject.SetActive(true);
	}
	public void ShowHistoricalGames() {
		Cleanup();
		HistoricalGamesRootElement.gameObject.SetActive(true);
	}

	public void Exit() {
		Application.Quit();
	}

	public void ViewGame(GameButtonBehaviour button) {
		Debug.Log($"Loading Game: {button.GameID}");
		gameRunner.GameIDFocus = button.GameID;
		SceneManager.LoadScene(Constants.SCENE_VIEWER);
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
}