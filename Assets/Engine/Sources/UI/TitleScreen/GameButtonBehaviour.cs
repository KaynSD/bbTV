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

	[SerializeField] protected TextMeshProUGUI headerText;
	[SerializeField] protected TextMeshProUGUI mainText;
	[SerializeField] protected TextMeshProUGUI footerText;

	[SerializeField] protected Image homeTeam; 
	[SerializeField] protected Image awayTeam;

	public Button self;

	[SerializeField] protected UICornersGradient gradient;

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

		BBTeam homeTeam = database.GetTeam(homeID);
		BBTeam awayTeam = database.GetTeam(awayID);

		string homeTeamName = homeTeam?.fullName ?? "Unknown Team";
		string awayTeamName = awayTeam?.fullName ?? "Unknown Team";

		string homeColorString = homeTeam?.mainColor ?? "#666666";
		string awayColorString = awayTeam?.mainColor ?? "#666666";

		Color homeColorColor = Color.gray, awayColorColor = Color.gray;
		ColorUtility.TryParseHtmlString(homeColorString, out homeColorColor);
		ColorUtility.TryParseHtmlString(awayColorString, out awayColorColor);
		Color homeColorAlpha = new Color(homeColorColor.r, homeColorColor.g, homeColorColor.b, 0.6f);
		Color awayColorAlpha = new Color(awayColorColor.r, awayColorColor.g, awayColorColor.b, 0.6f);

		gradient.m_topLeftColor = homeColorColor;
		gradient.m_topRightColor = awayColorColor;
		gradient.m_bottomLeftColor = homeColorAlpha;
		gradient.m_bottomRightColor = awayColorAlpha;
		

		headerText.text = $"{leagueName}, {season}, {day}; {series}"; 

		if(gameType == GameType.FORECAST) {
			mainText.text = $"{homeTeamName} <b>v</b> {awayTeamName}";
		} else {
			mainText.text = $"{homeTeamName} <b><color={homeColorString}>{newData.homeScore}</color> v <color={awayColorString}>{newData.awayScore}</color></b> {awayTeamName} ";
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
