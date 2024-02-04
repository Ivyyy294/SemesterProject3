using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class TeamWonLostUi : MonoBehaviour
{
	[SerializeField] GameObject panel;
	[SerializeField] GameObject wonUi;
	[SerializeField] GameObject lostUi;
	[SerializeField] GameEvent hidePlayerUiEvent;

	[Header ("Audio")]
	[SerializeField] AudioAsset audioWon;
	[SerializeField] AudioAsset audioLost;

	MatchScoreController scoreController;
	int localPlayerTeamIndex;

    // Start is called before the first frame update
    void Start()
    {
		scoreController = MatchController.Me.MatchScoreController;

		if (PlayerConfigurationManager.Me)
		{
			int localPlayerId = PlayerConfigurationManager.LocalPlayerId;
			localPlayerTeamIndex = PlayerConfigurationManager.Me.playerConfigurations[localPlayerId].teamNr;
		}
		else
			localPlayerTeamIndex = 0;

		panel.SetActive (false);
		wonUi.SetActive (false);
		lostUi.SetActive (false);
    }

	public void ShowScoreBoard()
	{
		if (!panel.activeInHierarchy)
		{
			hidePlayerUiEvent?.Raise();
			panel.SetActive (true);
		}

		bool won = scoreController.HasTeamWon (localPlayerTeamIndex);

		if (won && !wonUi.activeInHierarchy)
		{
			wonUi.SetActive (true);
			audioWon?.PlayOneShot();

		}
		else if (!won && !lostUi.activeInHierarchy)
		{
			lostUi.SetActive (!won);
			audioLost?.PlayOneShot();
		}
	}
}
