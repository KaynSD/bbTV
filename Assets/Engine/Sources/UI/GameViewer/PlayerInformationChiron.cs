using System;
using System.Collections;
using System.Collections.Generic;
using blaseball;
using blaseball.file;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class PlayerInformationChiron : MonoBehaviour
{
	[Inject] public IBlaseballFileLoader fileLoader;
	[SerializeField] protected Sprite HalfStarGraphic;
	[SerializeField] protected Sprite FullStarGraphic;
	[SerializeField] protected Image ImagePrefab;
	[SerializeField] protected TextMeshProUGUI PlayerNameText;
	[SerializeField] protected TextMeshProUGUI PlayerSecondLineText;
	[SerializeField] protected TextMeshProUGUI PlayerThirdLineText;
	[SerializeField] protected TextMeshProUGUI FirstStatText;
	[SerializeField] protected TextMeshProUGUI SecondStatText;
	[SerializeField] protected Transform firstStarContainer;
	[SerializeField] protected Transform secondStarContainer;
	[SerializeField] protected GameObject TloppsCardContainer;
	[SerializeField] protected Image TloppsCardImage;

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void ShowBatter(BBPlayer player, BBTeam team) {
		StartCoroutine(LoadTloppsCard(player));

		PlayerNameText.text = $"{player.name}";
		PlayerSecondLineText.text = $"Batting for the {team.nickname}";
		PlayerThirdLineText.text = "$???";

		Debug.Log($"Player Stats for {player.name} were {Helper.GetStarRating(BBPlayer.BatterRating(player))} and {Helper.GetStarRating(BBPlayer.BaserunningRating(player))}");

		FirstStatText.text = "Batting";
		SecondStatText.text = "Baserunning";
		ShowStars(BBPlayer.BatterRating(player), firstStarContainer);
		ShowStars(BBPlayer.BaserunningRating(player), secondStarContainer);
	}


	private void ShowStars(float v, Transform parentObj)
	{
		while(parentObj.childCount > 1) {
			Transform t = parentObj.GetChild(1);
			t.SetParent(null);
			Destroy(t.gameObject);
		}
		int halfStars = Mathf.RoundToInt(v * 10f);
		int stars = halfStars / 2; 
		bool halfStar = halfStars % 2 == 1;

		for(int i = 0; i < stars; i++) {
			Image newStar = Instantiate(ImagePrefab);
			newStar.transform.SetParent(parentObj);
			newStar.sprite = FullStarGraphic;
		}

		if(halfStar) {
			Image newStar = Instantiate(ImagePrefab);
			newStar.transform.SetParent(parentObj);
			newStar.sprite = HalfStarGraphic;
		}
	}

	private IEnumerator LoadTloppsCard(BBPlayer player)
	{
		UnityWebRequest www = new UnityWebRequest(fileLoader.GetPlayerTloppsCard(player.id));
		www.downloadHandler = new DownloadHandlerTexture();
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log($"Failed to load card for {player.name}");
			TloppsCardContainer.gameObject.SetActive(false);
		} else {
			TloppsCardContainer.gameObject.SetActive(true);
			Texture2D c = ((DownloadHandlerTexture)www.downloadHandler).texture;
			TloppsCardImage.sprite = Sprite.Create(
				c, 
				new Rect(0, 0, c.width, c.height), 
				new Vector2(0.5f, 0.5f),
				100
			);
			//TloppsCardImage.material = new Material(TloppsCardImage.material.shader);
			//TloppsCardImage.color = Color.white;
			//TloppsCardImage.material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
		}
	}
}
