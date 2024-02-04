using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class MatchGameOver : MonoBehaviour
{
	MatchTimer matchTimer;
	MatchScoreController scoreController;
	MatchPauseController pauseController;
	[SerializeField] AudioAsset audioGameOver;
	[SerializeField] GameEvent gameOverEvent;

	bool eventsRaised = false;

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
		if (GameOver() && !eventsRaised)
		{
			eventsRaised = true;
			gameOverEvent.Raise();
			pauseController.PauseMatch (true);
			audioGameOver.PlayOneShot();
		}
	}
}
