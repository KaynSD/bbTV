using System.Collections;
using System.Text.RegularExpressions;
using blaseball.service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace blaseball.ui {
	public class LoggingPanel : MonoBehaviour, IUILogger
	{

		[Inject] IBlaseballResultsService service;
		string textToAppend = "";
		public TextMeshProUGUI text;
		public ScrollRect scrollRect;
		public GameObject root;

		void Start() {
		}
		public void Log(string line) {
			textToAppend +=$"\n{line}";
		}

		public void Open() {
			root.SetActive(true);
		}

		public void Close() {
			root.SetActive(false);
		}

		void Update() {
			if(textToAppend == "") return;
			text.text += textToAppend;
			textToAppend = "";

			text.ForceMeshUpdate();

			StartCoroutine(ScrollAtEndOfNextFrame());

		}

		void OnDestroy() {
			Debug.Log("Disconnecting Service!");
			service.Disconnect();
		}

		public IEnumerator ScrollAtEndOfNextFrame() {
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			scrollRect.normalizedPosition = new Vector2(0, 0);
		}
	}
}