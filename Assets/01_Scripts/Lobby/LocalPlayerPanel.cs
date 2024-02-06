using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LocalPlayerPanel : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration;

	[SerializeField] TMP_InputField inputPlayerName;
	[SerializeField] Toggle toogleYAxis;

	[Header ("ReadyButton")]
	[SerializeField] Button readyButton;
	[SerializeField] Sprite normalSprite;
	[SerializeField] Sprite readySprite;

	[Header ("SelectTeam")]
	[SerializeField] TeamColorSettings teamColor;
	[SerializeField] Image team1;
	[SerializeField] Image team2;
	[Space]
	[SerializeField] Button buttonTeam1;
	[SerializeField] Button buttonTeam2;
	[Space]
	[SerializeField] Sprite buttonTeamNormalSprite;
	[SerializeField] Sprite buttonTeamSelectedSprite;

	public void OnReadyButtonPressed()
	{
		playerConfiguration.ready = !playerConfiguration.ready;
	}

	private void Start()
	{
		inputPlayerName.text = playerConfiguration.playerName;
		toogleYAxis.onValueChanged.AddListener (OnInvertAxisToggled);
		toogleYAxis.isOn = playerConfiguration.invertYAxis;

	}

	private void Update()
	{
		readyButton.image.sprite = playerConfiguration.ready ? readySprite : normalSprite;

		team1.color = teamColor.GetTeamColor (0);
		team2.color = teamColor.GetTeamColor (1);

		buttonTeam1.image.sprite = playerConfiguration.teamNr == 0 ? buttonTeamSelectedSprite : buttonTeamNormalSprite;
		buttonTeam2.image.sprite = playerConfiguration.teamNr == 1 ? buttonTeamSelectedSprite : buttonTeamNormalSprite;
	}

	public void OnNameChanged (string newName)
	{
		playerConfiguration.playerName = inputPlayerName.text;
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void SetTeamIndex (int teamNr)
	{
		playerConfiguration.teamNr = teamNr;
	}

	public void OnInvertAxisToggled (bool isOn)
	{
		playerConfiguration.invertYAxis = isOn;
	}
}
