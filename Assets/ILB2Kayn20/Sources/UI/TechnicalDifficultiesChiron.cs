using System.Collections;
using System.Collections.Generic;
using blaseball.vo;
using TMPro;
using UnityEngine;

public class TechnicalDifficultiesChiron : MonoBehaviour
{
	public TextMeshProUGUI DisplayText;
	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
	public void ShowText(string text) {
		DisplayText.text = $"Technical Difficulties:\n{text}";
	}
}
