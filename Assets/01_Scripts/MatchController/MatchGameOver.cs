using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchGameOver : MonoBehaviour
{
	MatchTimer matchTimer;
	MatchScoreController scoreController;
	MatchPauseController pauseController;

	public bool GameOver()
	{
		bool timerUp = matchTimer.TimeRemaining == 0f;
		bool tie = scoreController.Tie;

		return timerUp && !tie;
	}

    // Start is called before the first frame update
    void Start()
    {
        matchTimer = MatchController.Me.MatchTimer;
		scoreController = MatchController.Me.MatchScoreController;
		pauseController = MatchController.Me.MatchPauseController;
    }

	private void Update()
	{
		if (GameOver() && !pauseController.IsMatchPaused)
			pauseController.PauseMatch (true);
	}
}
