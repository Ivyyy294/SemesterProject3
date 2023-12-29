using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerThrowBall : NetworkBehaviour
{
	[SerializeField] float throwForce = 10f;
	Ball ball;
	short playerID;
    // Start is called before the first frame update
    void Start()
    {
        ball = Ball.Me;
		playerID = GetComponentInChildren <PlayerID>().PlayerId;
    }

    // Update is called once per frame
    void Update()
    {
        if (Owner && PlayerHasBall())
		{
			if (Input.GetMouseButtonDown (1))
			{
				if (Host)
					ThrowBall();
				else
					InvokeRPC ("ThrowBall");
			}
		}
    }

	bool PlayerHasBall()
	{
		return ball != null && ball.CurrentPlayerId == playerID;
	}

	protected override void SetPackageData()
	{
	}

	[RPCAttribute]
	public void ThrowBall()
	{
		if (Host && ball)
			ball.Throw (transform.position + transform.forward * 2f, transform.forward * throwForce);
	}
}
