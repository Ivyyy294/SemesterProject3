using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallStatus : MonoBehaviour
{
	[Header("Lara Values")]
	[SerializeField] PlayerStatus playerStatus;
	[SerializeField] PlayerID playerIDObj;

	short playerID;
	Ball ball;

	//Private Methods
    // Start is called before the first frame update
    void Start()
    {
		ball = Ball.Me;
		playerID = playerIDObj.PlayerId;
    }

	private void Update()
	{
		bool playerHasBall = ball != null && ball.CurrentPlayerId == playerID;

		if (playerStatus.CheckStatusTyp (PlayerStatus.StatusTyp.HAS_BALL) != playerHasBall)
			playerStatus.SetStatusTyp (PlayerStatus.StatusTyp.HAS_BALL, playerHasBall);
	}
}
