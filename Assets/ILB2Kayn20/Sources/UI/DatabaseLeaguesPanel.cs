using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.vo;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseLeaguesPanel : MonoBehaviour
{

	public TMPro.TextMeshProUGUI title;
	public Button SampleButton;
	public GameObject SampleGroup;

	void Start() {
		SampleGroup.gameObject.SetActive(false);
		SampleButton.gameObject.SetActive(false);
	} 
	public void ShowSubleagues(IBlaseballDatabase database) {
		BBLeague league = database.GetLeague();
		title.text = league.name;
		//
		string[] subleagues = league.subleagues;
		foreach (string subleagueID in subleagues) {
			BBSubleague subleague = database.GetSubleague(subleagueID);
			GameObject newDisplay = Instantiate(SampleGroup);
			newDisplay.transform.SetParent(transform);

			newDisplay.SetActive(true);
			newDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = subleague.name;

			foreach(string divisionID in subleague.divisions) {
				BBDivision division = database.GetDivision(divisionID);
				GameObject newDivision = Instantiate(SampleGroup);
				newDivision.transform.SetParent(newDisplay.transform);
				
				newDivision.SetActive(true);
				newDivision.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = division.name;
				foreach(string teamID in division.teams) {
					BBTeam team = database.GetTeam(teamID);
					Button TeamButton = Instantiate(SampleButton.gameObject).GetComponent<Button>();
					TeamButton.transform.SetParent(newDivision.transform);

					TMPro.TextMeshProUGUI text = TeamButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
					text.text = team.fullName;
					Color teamColor = Color.black;
					Color teamSecond = Color.white;
					ColorUtility.TryParseHtmlString(team.mainColor, out teamColor);
					ColorUtility.TryParseHtmlString(team.secondaryColor, out teamSecond);
					text.color = teamColor;

					ColorBlock colorBlock = TeamButton.colors;
					colorBlock.normalColor = teamSecond;
					float h, s, v;
					Color.RGBToHSV(teamSecond, out h, out s, out v);
					colorBlock.highlightedColor = Color.HSVToRGB(h, s, (1 + v) / 2);
					colorBlock.pressedColor = Color.HSVToRGB(h, s, v / 2);
					colorBlock.disabledColor = Color.HSVToRGB(h, s, v / 4);
					colorBlock.selectedColor = teamSecond;
					TeamButton.colors = colorBlock;

				}
			}

		}
	}

}
