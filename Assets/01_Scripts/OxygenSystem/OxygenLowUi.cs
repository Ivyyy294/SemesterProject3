using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenLowUi : MonoBehaviour
{
	[Range (0, 100)]
	[SerializeField] float oxygenLowThresholdPercent; 
    [SerializeField] GameObject uiObj;
	PlayerOxygen playerOxygen;
	MatchGameOver gameOver;
	AudioPlayer audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
		playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
		gameOver = MatchController.Me.MatchGameOver;
		audioPlayer = GetComponent <AudioPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
		if (gameOver.GameOver())
		{
			uiObj.SetActive (false);
			audioPlayer.FadeOut (0.5f);
			return;
		}

        if (uiObj && playerOxygen)
		{
			bool showWarning = playerOxygen.CurrentOxygenPercent <= oxygenLowThresholdPercent;

			if (showWarning && !uiObj.activeInHierarchy)
			{
				audioPlayer.Play();
				uiObj.SetActive (true);
			}
			else if (!showWarning && uiObj.activeInHierarchy)
				uiObj.SetActive (false);
		}
    }
}
