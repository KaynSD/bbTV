using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewindPanel : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public delegate void SliderDelegate(int value);

	public SliderDelegate OnChanged;
	private Slider slider;

	int currentValue;
	int settingValue;

	void Awake() {
		
		slider = GetComponentInChildren<Slider>();
		currentValue = settingValue = (int)slider.value;
	}

	public void OnSelect(BaseEventData eventData)
	{
		Debug.Log("Select");
		settingValue = currentValue;
	}

	internal void Show()
	{
		gameObject.SetActive(true);
	}

	internal void Disable()
	{
		gameObject.SetActive(false);
	}

	public void OnChange(float value) {
		Debug.Log("Change");
		settingValue = (int)value;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Debug.Log("Deselect");
		if(settingValue != currentValue) OnChanged?.Invoke(settingValue);
		currentValue = settingValue;
	}

	public void SetLength(int max, int current = -1) {
		slider.minValue = 0;
		slider.maxValue = max;
		slider.SetValueWithoutNotify(current);
	}

}
