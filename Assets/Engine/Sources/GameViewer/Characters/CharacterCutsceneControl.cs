using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterCutsceneControl : MonoBehaviour
{
	[Header("Reference")]
	public Transform LeftHandAttachmentReference;
	public Transform RightHandAttachmentReference;
	public Transform HelmetAttachmentReference;
	public Animator animator => GetComponent<Animator>();

	public abstract void SetPlayerName(string playerName);
	public abstract void SetPrimaryColor(Color color);
	public abstract void SetSecondaryColor(Color color);
}
