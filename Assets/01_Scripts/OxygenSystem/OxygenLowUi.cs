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

    // Start is called before the first frame update
    void Start()
    {
		playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
		gameOver = MatchController.Me.MatchGameOver;
    }

    // Update is called once per frame
    void Update()
    {
		if (gameOver.GameOver())
		{
			uiObj.SetActive (false);
			return;
		}

        if (uiObj && playerOxygen)
		{
			bool showWarning = playerOxygen.CurrentOxygenPercent <= oxygenLowThresholdPercent;
			uiObj.SetActive (showWarning);
		}
    }
}
