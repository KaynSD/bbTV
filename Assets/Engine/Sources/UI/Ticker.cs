using System.Collections;
using System.Collections.Generic;
using blaseball.runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Ticker : MonoBehaviour
{
	[Inject] public BBAnnouncements announcements;
	public float speed;
	[SerializeField] protected TextMeshProUGUI textSample;
	[SerializeField] protected RectTransform star;

	private List<RectTransform> layoutElements;
	private bool spacerToggle = false;

	private RectTransform rect;
	void Start() {
		layoutElements = new List<RectTransform>();
		rect = (RectTransform)transform;
		textSample.gameObject.SetActive(false);
	}

	void Update() {
		float w = 0;
		if(layoutElements.Count > 0) w = layoutElements[0].transform.position.x;

		for(int i = 0; i < layoutElements.Count; i++) {
			RectTransform e = layoutElements[i];
			w += e.rect.width;
		}

		if(w < Screen.width && layoutElements.Count < 100) {
			RectTransform newElement;
			if(spacerToggle) {
				Image e = Instantiate(star).GetComponent<Image>();
				e.transform.SetParent(rect);
				newElement = (RectTransform)e.transform;
			} else {
				TextMeshProUGUI t = Instantiate(textSample).GetComponent<TextMeshProUGUI>();
				t.text = announcements.GetAnnouncement(false);
				t.gameObject.SetActive(true);
				t.transform.SetParent(rect);
				newElement = (RectTransform)t.transform;
			}

			layoutElements.Add(newElement);
			newElement.transform.position = new Vector2(w, 0);

			spacerToggle = !spacerToggle;
		}

		foreach(RectTransform t in layoutElements) {
			t.transform.position = new Vector2(t.position.x - Time.deltaTime * speed, t.position.y);
		}

		bool deleted = false;
		do {
			deleted = false;
			if(layoutElements.Count == 0) break;
			
			RectTransform t = layoutElements[0];
			if(t.position.x + t.rect.width < 0) {
				Destroy(t.gameObject);
				layoutElements.RemoveAt(0);
				deleted = true;
			} 
		} while(deleted);

	}
}
