using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Ivyyy.Utils;

public class MatchTimerUi : MonoBehaviour
{
	[Header ("Audio")]
	[SerializeField] AudioAsset audioAsset;

	[Header ("Lara values")]
	[SerializeField] TextMeshProUGUI labelTimer;
	[SerializeField] GameObject labelSuddenDeath;
	MatchTimer matchTimer;
	MatchScoreController matchScoreController;
	BitSet audioMemory = new BitSet (1);

    // Start is called before the first frame update
    void Start()
    {
        matchTimer = MatchController.Me.MatchTimer;
		matchScoreController = MatchController.Me.MatchScoreController;
    }

    // Update is called once per frame
    void Update()
    {
        labelTimer.text = GetRemainingTimeAsString();

		labelSuddenDeath.SetActive (matchTimer.TimeRemaining <= 0f && matchScoreController.Tie);

		HandleAudio();
    }

	private string GetRemainingTimeAsString()
	{
		float remaining = matchTimer.TimeRemaining;
		int minutes = (int) (remaining / 60f);
		int seconds = (int) (remaining % 60f);

		if (seconds > 9)
			return minutes + ":" + seconds;
		else
			return minutes + ":0" + seconds;
	}

	private void HandleAudio()
	{
		float timeLeft = matchTimer.TimeRemaining;

		if (timeLeft <= 1f)
			PlayAudio (5);
		else if (timeLeft <= 2f)
			PlayAudio (4);
		else if (timeLeft <= 3f)
			PlayAudio (3);
		else if (timeLeft <= 10f)
			PlayAudio (2);
		else if (timeLeft <= 30f)
			PlayAudio (1);
		else if (timeLeft <= 60f)
			PlayAudio (0);
	}

	private void PlayAudio (int memNr)
	{
		if (!audioMemory.Check (memNr))
		{
			audioAsset.Play ();
			audioMemory.SetBit (memNr, true);
		}
	}
}
