using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerStatus))]
public class PlayerBallStatus : MonoBehaviour
{
	PlayerStatus playerStatus;
	short playerID;
	Ball ball;

	//Private Methods
    // Start is called before the first frame update
    void Start()
    {
		playerStatus = GetComponent <PlayerStatus>();
		ball = Ball.Me;
		playerID = GetComponentInChildren <PlayerID>().PlayerId;
    }

	private void Update()
	{
		bool playerHasBall = ball != null && ball.CurrentPlayerId == playerID;

		if (playerStatus.CheckStatusTyp (PlayerStatus.StatusTyp.HAS_BALL) != playerHasBall)
			playerStatus.SetStatusTyp (PlayerStatus.StatusTyp.HAS_BALL, playerHasBall);
	}
}
