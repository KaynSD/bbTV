using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DefaultCharacterCutsceneControl : CharacterCutsceneControl
{
	[Header("DefaultCCC References")]
	public SkinnedMeshRenderer PrimaryColorSMR;
	public SkinnedMeshRenderer SecondaryColorSMR;
	public TextMeshProUGUI HeaderText;

	public override void SetPlayerName(string playerName)
	{
		HeaderText.text = playerName;
	}

	public override void SetPrimaryColor(Color color)
	{
		HeaderText.color = color;
		PrimaryColorSMR.material = new Material(PrimaryColorSMR.material.shader);
		PrimaryColorSMR.material.color = color;
	}

	public override void SetSecondaryColor(Color color)
	{
		SecondaryColorSMR.material = new Material(PrimaryColorSMR.material.shader);
		SecondaryColorSMR.material.color = color;
	}
}
