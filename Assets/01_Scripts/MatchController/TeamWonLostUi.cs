using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamWonLostUi : MonoBehaviour
{
	[SerializeField] GameObject wonUi;
	[SerializeField] GameObject lostUi;

	[Header ("Audio")]
	[SerializeField] AudioAsset audioWon;
	[SerializeField] AudioAsset audioLost;

	MatchGameOver matchGameOver;
	MatchScoreController scoreController;
	int localPlayerTeamIndex;

    // Start is called before the first frame update
    void Start()
    {
        matchGameOver = MatchController.Me.MatchGameOver;
		scoreController = MatchController.Me.MatchScoreController;

		if (PlayerConfigurationManager.Me)
		{
			int localPlayerId = PlayerConfigurationManager.LocalPlayerId;
			localPlayerTeamIndex = PlayerConfigurationManager.Me.playerConfigurations[localPlayerId].teamNr;
		}
		else
			localPlayerTeamIndex = 0;

		wonUi.SetActive (false);
		lostUi.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
        if (matchGameOver.GameOver())
		{
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
}
