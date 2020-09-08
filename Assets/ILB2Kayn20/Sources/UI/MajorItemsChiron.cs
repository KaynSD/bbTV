using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.vo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MajorItemsChiron : MonoBehaviour
{
	[SerializeField] protected Sprite OnLightImage;
	[SerializeField] protected Sprite OffLightImage;
	[SerializeField] protected Sprite OnBaseImage;
	[SerializeField] protected Sprite OffBaseImage;
	[SerializeField] protected GameObject textPanel;
	[SerializeField] protected GameObject majorItemsPanel;
	[SerializeField] protected TextMeshProUGUI text;
	[SerializeField] protected Image base1;
	[SerializeField] protected Image base2;
	[SerializeField] protected Image base3;
	[SerializeField] protected Image strike1;
	[SerializeField] protected Image strike2;
	[SerializeField] protected Image strike3;
	[SerializeField] protected Image out1;
	[SerializeField] protected Image out2;
	[SerializeField] protected Image out3;
	[SerializeField] protected Image ball1;
	[SerializeField] protected Image ball2;
	[SerializeField] protected Image ball3;
	[SerializeField] protected Image ball4;

	public void Start(){
		SetOuts(0);
		SetBalls(0);
		SetStrikes(0);
		ShowBase1(false);
		ShowBase2(false);
		ShowBase3(false);
	}
	public void Show() {
		gameObject.SetActive(true);
		SetText("");
	}

	public void Hide() {
		gameObject.SetActive(false);
		SetText("");
	}

	public void SetText(string newText, bool animate = false) {
		if(animate) textPanel.SetActive(true);
		text.text = newText;
	}

	public void ShowBase1(bool active, bool animate = false) {
		base1.sprite = active ? OnBaseImage : OffBaseImage;
	}
	public void ShowBase2(bool active, bool animate = false) {
		base2.sprite = active ? OnBaseImage : OffBaseImage;
	}
	public void ShowBase3(bool active, bool animate = false) {
		base3.sprite = active ? OnBaseImage : OffBaseImage;
	}

	public void SetOuts(int outs, bool animate = false) {
		out1.sprite = outs >= 1 ? OnLightImage : OffLightImage;
		out2.sprite = outs >= 2 ? OnLightImage : OffLightImage;
		out3.sprite = outs >= 3 ? OnLightImage : OffLightImage;
	}

	public void ToggleText(bool v)
	{
		textPanel.SetActive(v);
	}

	public void ToggleMajorItems(bool v)
	{
	}

	public void SetBalls(int balls, bool animate = false) {
		ball1.sprite = balls >= 1 ? OnLightImage : OffLightImage;
		ball2.sprite = balls >= 2 ? OnLightImage : OffLightImage;
		ball3.sprite = balls >= 3 ? OnLightImage : OffLightImage;
		ball4.sprite = balls >= 4 ? OnLightImage : OffLightImage;
	}

	public void SetStrikes(int strikes, bool animate = false) {
		strike1.sprite = strikes >= 1 ? OnLightImage : OffLightImage;
		strike2.sprite = strikes >= 2 ? OnLightImage : OffLightImage;
		strike3.sprite = strikes >= 3 ? OnLightImage : OffLightImage;
	}
}
