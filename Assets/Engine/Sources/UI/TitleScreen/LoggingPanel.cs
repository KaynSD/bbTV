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

		[Inject] public IBlaseballResultsService service;
		string textToAppend = "";
		public TextMeshProUGUI text;
		public ScrollRect scrollRect;
		public GameObject root;

		public bool IsActive {get; protected set;}
		void Start() {
			Close();
		}
		public void Log(string line) {
			textToAppend +=$"\n{line}";
		}

		public void Open() {
			root.SetActive(true);
			IsActive = true;
		}

		public void Close() {
			root.SetActive(false);
			IsActive = false;
		}

		void Update() {
			if(Input.GetKeyDown(KeyCode.L)) {
				if(IsActive) {
					Close();
				} else{
					Open();
				}
			}

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