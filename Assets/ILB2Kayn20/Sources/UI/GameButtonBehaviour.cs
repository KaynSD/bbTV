using System;
using System.Collections;
using System.Collections.Generic;
using blaseball;
using blaseball.db;
using blaseball.file;
using blaseball.runtime;
using blaseball.service;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class GameButtonBehaviour : MonoBehaviour
{
	[Inject] public IBlaseballDatabase database;
	[Inject] public IBlaseballResultsService service;
	[Inject] public GameRunner gameRunner;
	[Inject] public IBlaseballFileLoader fileLoader;

	public TextMeshProUGUI headerText;
	public TextMeshProUGUI mainText;
	public TextMeshProUGUI footerText;

	public Image homeTeam; 
	public Image awayTeam;

	public Button self;

	public enum GameType {
		HISTORICAL,
		FORECAST,
		CURRENT
	}
	public GameType gameType;

	public string GameID = "";
	
	void Start() {
		self = GetComponent<Button>();
		
	}

	bool setup = false;
	void Update() {
		if(!setup && service != null) {

			foreach(KeyValuePair<string, BBGame> kvp in gameRunner.Games) {
				if(kvp.Key == GameID) {
					service.OnGameUpdateRecieved += GameUpdate;
					StartCoroutine(SetupImages(kvp.Value.current.homeTeam, kvp.Value.current.awayTeam));
					GameUpdate(kvp.Value.current);
					setup = true;
					return;
				}
			}
			
		} else {
			if(service == null)Debug.Log("Service is still null!");
		}
	}

	IEnumerator SetupImages(string homeID, string awayID) {
		UnityWebRequest www = new UnityWebRequest(fileLoader.GetTeamTexturePath(homeID));
		www.downloadHandler = new DownloadHandlerTexture();
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			// No op, but error with home team
		} else {
			homeTeam.material = new Material(homeTeam.material.shader);
			homeTeam.color = Color.white;
			homeTeam.material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
		}

		www = new UnityWebRequest(fileLoader.GetTeamTexturePath(awayID));
		www.downloadHandler = new DownloadHandlerTexture();
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			// No op, but error with home team
		} else {
			awayTeam.material = new Material(homeTeam.material.shader);
			awayTeam.color = Color.white;
			awayTeam.material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
		}
	}

	void GameUpdate(BBGameState newData) {


		// Only interested in my own data...
		if(newData.id != GameID) return;

		switch(gameType) {
			case GameType.FORECAST:
			if(newData.gameStart) {
				Debug.Log($"Forecast game {GameID} has started!");
				DestroySelf();
				return;
			}
			break;

			case GameType.CURRENT:
			if(newData.gameComplete)  {
				Debug.Log($"Current game {GameID} has ended!");
				DestroySelf();
				return;
			}
			break;

			case GameType.HISTORICAL:
			default:
			break;
		}
		if(service == null)Debug.Log("Service is still null!");

		string leagueName = database.GetLeague().name;

		string season = $"Season {newData.season + 1}";
		string day = $"Day {newData.day + 1}";

		string series = "";

		string homeID = newData.homeTeam;
		string awayID = newData.awayTeam;

		string homeTeam = database.GetTeam(homeID)?.fullName ?? "Unknown Team";
		string awayTeam = database.GetTeam(awayID)?.fullName ?? "Unknown Team";

		string homeColor = database.GetTeam(homeID)?.mainColor ?? "0x666666";
		string awayColor = database.GetTeam(awayID)?.mainColor ?? "0x666666";

		headerText.text = $"{leagueName}, {season}, {day}; {series}"; 

		if(gameType == GameType.FORECAST) {
			mainText.text = $"{homeTeam} <b>v</b> {awayTeam}";
		} else {
			mainText.text = $"{homeTeam} <b><color={homeColor}>{newData.homeScore}</color> v <color={awayColor}>{newData.awayScore}</color></b> {awayTeam} ";
		}

		string footerTextChange =  $"Weather: {Helper.GetWeather(newData.weather)}";
		if(gameType == GameType.CURRENT) {
			string inningPos = newData.topOfInning ? "(top)" : "(bottom)";
			footerTextChange += $", {newData.inning + 1} Inning {inningPos}";
		}

		if(gameType != GameType.HISTORICAL) {
			string favoured = "";
			int homeOdds = Mathf.RoundToInt(newData.homeOdds * 100f);
			int awayOdds = Mathf.RoundToInt(newData.awayOdds * 100f);
			if(homeOdds == awayOdds) {
				favoured = "Evens";
			} else {
				int odds = 0;
				if (homeOdds > awayOdds) {
					favoured = database.GetTeam(homeID)?.nickname ?? "Unknown";
					odds = homeOdds;
				} else {
					favoured = database.GetTeam(awayID)?.nickname ?? "Unknown";
					odds = awayOdds;
				}

				favoured += $" {odds}%";

			}
			footerTextChange += $", Favoured: {favoured}";
		}
		footerText.text = footerTextChange;

		//StartCoroutine("DelayThenRefresh");
	}

	IEnumerator DelayThenRefresh() {
		yield return new WaitForEndOfFrame();
		//yield return new WaitForEndOfFrame();
		((RectTransform)transform).ForceUpdateRectTransforms();
		//((RectTransform)transform.parent).ForceUpdateRectTransforms();
		//((RectTransform)transform.parent.parent).ForceUpdateRectTransforms();
		//((RectTransform)transform.parent.parent.parent).ForceUpdateRectTransforms();
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
		Destroy(this);
	}

	void OnDestroy(){
		service.OnGameUpdateRecieved -= GameUpdate;
	}
}
