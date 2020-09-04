using System.Collections;
using System.Collections.Generic;
using blaseball;
using blaseball.vo;
using TMPro;
using UnityEngine;

public class PlayerInformationChiron : MonoBehaviour
{
	public TextMeshProUGUI PlayerNameText;
	public TextMeshProUGUI PlayerStatsHeaderText;
	public TextMeshProUGUI PlayerStatsValueText;

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void ShowBatterText(BBPlayer player, BBTeam team) {
		PlayerNameText.text = $"{player.name}\nBatting now for the {team.fullName}";
		PlayerStatsHeaderText.text = "Batting\nBaserunning";
		PlayerStatsValueText.text = $"{Helper.GetStarRating(BBPlayer.BatterRating(player))}\n{Helper.GetStarRating(BBPlayer.BaserunningRating(player))}";
	}
}
