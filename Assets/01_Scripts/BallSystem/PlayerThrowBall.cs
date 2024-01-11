using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerThrowBall : NetworkBehaviour
{
	[SerializeField] float throwForce = 10f;
	[SerializeField] Transform ballSpawn;

	[Header ("Lara Values")]
	[SerializeField] GameObject ballGhost;
	PlayerOxygen playerOxygen;
	PlayerBallStatus playerBallStatus;

	//Private
	Ball ball;

    // Start is called before the first frame update
    void Start()
    {
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();
        ball = Ball.Me;
    }

    // Update is called once per frame
    void Update()
    {
		bool hasBall = playerBallStatus.HasBall();

		if (ballGhost.activeInHierarchy != hasBall)
			ballGhost.SetActive (hasBall);

        if (Owner && hasBall)
		{
			if (playerOxygen.OxygenEmpty)
				DropBall();
			else if (Input.GetMouseButtonDown (0))
				ThrowBall();
		}
    }

	protected override void SetPackageData()
	{
	}

	[RPCAttribute]
	public void ThrowBall()
	{
		if (Host)
			ball.Throw (ballSpawn.position, transform.forward * throwForce);
		else
			InvokeRPC ("ThrowBall");
	}

	[RPCAttribute]
	public void DropBall()
	{
		if (Host)
			ball.BallDrop (ballSpawn.position);
		else
			InvokeRPC("DropBall");
	}
}
