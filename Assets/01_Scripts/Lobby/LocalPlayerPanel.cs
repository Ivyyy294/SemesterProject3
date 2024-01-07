using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.UI;
using TMPro;

public class LocalPlayerPanel : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration;

	[SerializeField] GameObject readyButton;
	[SerializeField] TextMeshProUGUI labelReady;
	[SerializeField] TMP_InputField inputPlayerName;

	public void OnReadyButtonPressed()
	{
		playerConfiguration.ready = true;
	}

	private void Start()
	{
		inputPlayerName.text = playerConfiguration.playerName;
	}

	private void Update()
	{
		readyButton.SetActive (!playerConfiguration.ready);
		labelReady.gameObject.SetActive (playerConfiguration.ready);
	}

	public void OnNameChanged (string newName)
	{
		playerConfiguration.playerName = inputPlayerName.text;
	}

	public void SetTeamIndex (int teamNr)
	{
		playerConfiguration.teamNr = teamNr;
	}
}
