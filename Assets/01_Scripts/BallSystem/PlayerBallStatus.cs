using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallStatus : MonoBehaviour
{
	Ball ball;
	PlayerConfigurationContainer playerConfigurationContainer;

	public bool HasBall() {return ball != null && ball.CurrentPlayerId == playerConfigurationContainer.PlayerID;}

	//Private Methods
    // Start is called before the first frame update
    void Start()
    {
		ball = Ball.Me;
		playerConfigurationContainer = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>();
    }
}
