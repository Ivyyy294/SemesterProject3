using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamWonLostUi : MonoBehaviour
{
	[SerializeField] GameObject wonUi;
	[SerializeField] GameObject lostUi;

	MatchScoreController scoreController;
	MatchTimer matchTimer;
	int localPlayerTeamIndex;

    // Start is called before the first frame update
    void Start()
    {
        scoreController = MatchController.Me.MatchScoreController;
		matchTimer = MatchController.Me.MatchTimer;

		if (PlayerConfigurationManager.Me)
		{
			int localPlayerId = PlayerConfigurationManager.Me.LocalPlayerId;
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
        if (matchTimer.GameOver)
		{
			bool won = (localPlayerTeamIndex == 0 && scoreController.PointsTeam1 > scoreController.PointsTeam2)
				|| (localPlayerTeamIndex == 1 && scoreController.PointsTeam2 > scoreController.PointsTeam1);

			wonUi.SetActive (won);
			lostUi.SetActive (!won);
		}
    }
}
