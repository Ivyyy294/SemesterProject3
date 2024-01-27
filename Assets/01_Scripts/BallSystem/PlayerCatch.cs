using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatch : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	PlayerConfigurationContainer playerConfigurationContainer;
	PlayerAudio playerAudio;

	public bool CanCatchBall { get { return !playerOxygen.OxygenEmpty/* && (playerBlock.IsBlocking || !diverInput.DashPressed)*/;} }

	public void Catch()
	{
		if (Ball.Me)
		{
			Ball.Me.SetPlayerId (playerConfigurationContainer.PlayerID);
			playerAudio.PlayAudioCatch();
		}
		else
			Debug.Log ("No active ball!");
	}

    // Start is called before the first frame update
    void Start()
    {
        playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>();
		playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
    }
}
