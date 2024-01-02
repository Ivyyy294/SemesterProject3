using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallStatus : MonoBehaviour
{
	short playerID;
	Ball ball;

	//Public Methods
	public bool PlayerHasBall()
	{
		return ball != null && ball.CurrentPlayerId == playerID;
	}

	//Private Methods
    // Start is called before the first frame update
    void Start()
    {
		ball = Ball.Me;
		playerID = GetComponentInChildren <PlayerID>().PlayerId;
    }
}
