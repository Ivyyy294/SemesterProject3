using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MatchTimerUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI labelTimer;
	MatchTimer matchTimer;

    // Start is called before the first frame update
    void Start()
    {
        matchTimer = MatchController.Me.MatchTimer;
    }

    // Update is called once per frame
    void Update()
    {
        labelTimer.text = GetRemainingTimeAsString();
    }

	private string GetRemainingTimeAsString()
	{
		float remaining = matchTimer.TimeRemaining;
		int minutes = (int) (remaining / 60f);
		int seconds = (int) (remaining % 60f);
		return minutes + ":" + seconds;
	}
}
