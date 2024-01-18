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
	[SerializeField] float initalCooldown = 0.1f;
	PlayerOxygen playerOxygen;
	PlayerBallStatus playerBallStatus;
	PlayerInput playerInput;

	//Private
	Ball ball;
	float timer;

    // Start is called before the first frame update
    void Start()
    {
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();
		playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
        ball = Ball.Me;
    }

    // Update is called once per frame
    void Update()
    {
		bool hasBall = playerBallStatus.HasBall();

		if (ballGhost.activeInHierarchy != hasBall)
			ballGhost.SetActive (hasBall);

		//Prevent ball from being thrown instantly
		if (!hasBall)
			timer = 0f;
		else if (timer < initalCooldown)
			timer += Time.deltaTime;

        if (Owner && hasBall && timer > initalCooldown)
		{
			if (playerOxygen.OxygenEmpty)
				DropBall();
			else if (playerInput.ThrowPressed)
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
