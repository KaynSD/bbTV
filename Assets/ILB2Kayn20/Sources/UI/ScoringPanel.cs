using System.Collections;
using blaseball.file;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class ScoringPanel : MonoBehaviour
{
	[Inject] public IBlaseballFileLoader fileLoader;
	[SerializeField] protected Image HomeTeamImage;
	[SerializeField] protected Image AwayTeamImage;
	[SerializeField] protected TextMeshProUGUI HomeScore;
	[SerializeField] protected TextMeshProUGUI AwayScore;
	[SerializeField] protected TextMeshProUGUI Message;

	[SerializeField] protected Image TopOfInningsLight;
	[SerializeField] protected Image BottomOfInningsLight;

	protected bool firstTime = true;

	void Start() {
		SetTopOfInnings(false);
		SetBottomOfInnings(false);
		HomeScore.text = "--";
		AwayScore.text = "--";
		SetMessage("");
	}

	public void SetTopOfInnings(bool On) {
		TopOfInningsLight.color = On ? Color.red : Color.black;
	}
	public void SetBottomOfInnings(bool On) {
		BottomOfInningsLight.color = On ? Color.red : Color.black;
	}

	public void SetHomeScore(int value) {
		HomeScore.text = value.ToString();
	}
	public void SetAwayScore(int value) {
		AwayScore.text = value.ToString();
	}
	public void SetMessage (string message) {
		Message.text = message;
	}

	public void Setup(BBGameState gameState, bool animateScore = false) {
		SetTopOfInnings(gameState.gameComplete ? false : gameState.topOfInning);
		SetBottomOfInnings(gameState.gameComplete ? false :!gameState.topOfInning);
		SetHomeScore(gameState.homeScore);
		SetAwayScore(gameState.awayScore);
		SetMessage($"{gameState.inning + 1} INNING");

		if(firstTime) {
			StartCoroutine(SetupImagesCoroutine(gameState.homeTeam, gameState.awayTeam));
			firstTime = false;
		}
	}
	protected IEnumerator SetupImagesCoroutine(string homeID, string awayID) {
		UnityWebRequest www = new UnityWebRequest(fileLoader.GetTeamTexturePath(homeID));
		www.downloadHandler = new DownloadHandlerTexture();
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			// No op, but error with home team
		} else {
			HomeTeamImage.material = new Material(HomeTeamImage.material.shader);
			HomeTeamImage.color = Color.white;
			HomeTeamImage.material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
		}

		www = new UnityWebRequest(fileLoader.GetTeamTexturePath(awayID));
		www.downloadHandler = new DownloadHandlerTexture();
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			// No op, but error with home team
		} else {
			AwayTeamImage.material = new Material(AwayTeamImage.material.shader);
			AwayTeamImage.color = Color.white;
			AwayTeamImage.material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
		}
	}


}
