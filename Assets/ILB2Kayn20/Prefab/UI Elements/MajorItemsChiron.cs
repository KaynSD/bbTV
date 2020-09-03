using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MajorItemsChiron : MonoBehaviour
{
	public GameObject textPanel;
	public GameObject majorItemsPanel;
	public TextMeshProUGUI text;
	public Image base1;
	public Image base2;
	public Image base3;
	public Image strike1;
	public Image strike2;
	public Image out1;
	public Image out2;
	public Image ball1;
	public Image ball2;
	public Image ball3;

	public void Start(){
		SetText("");
		SetOuts(0);
		SetBalls(0);
		SetStrikes(0);
		ShowBase1(false);
		ShowBase2(false);
		ShowBase3(false);
	}
	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void SetText(string newText, bool animate = false) {
		text.text = newText;
	}


	public void ShowBase1(bool active, bool animate = false) {
		base1.gameObject.SetActive(active);
	}
	public void ShowBase2(bool active, bool animate = false) {
		base2.gameObject.SetActive(active);
	}
	public void ShowBase3(bool active, bool animate = false) {
		base3.gameObject.SetActive(active);
	}

	public void SetOuts(int outs, bool animate = false) {
		out1.gameObject.SetActive(outs >= 1);
		out2.gameObject.SetActive(outs >= 2);
	}

	public void ToggleText(bool v)
	{
	}

	public void ToggleMajorItems(bool v)
	{
	}

	public void SetBalls(int balls, bool animate = false) {
		ball1.gameObject.SetActive(balls >= 1);
		ball2.gameObject.SetActive(balls >= 2);
		ball3.gameObject.SetActive(balls >= 3);
	}

	public void SetStrikes(int strikes, bool animate = false) {
		strike1.gameObject.SetActive(strikes >= 1);
		strike2.gameObject.SetActive(strikes >= 2);
	}
}
