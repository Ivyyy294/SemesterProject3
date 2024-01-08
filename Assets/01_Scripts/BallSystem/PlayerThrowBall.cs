using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerThrowBall : NetworkBehaviour
{
	[SerializeField] float throwForce = 10f;

	[Header ("Lara Values")]
	[SerializeField] GameObject ballGhost;
	[SerializeField] PlayerOxygen playerOxygen;
	[SerializeField] PlayerStatus playerStatus;

	//Private
	Ball ball;

    // Start is called before the first frame update
    void Start()
    {
        ball = Ball.Me;
    }

    // Update is called once per frame
    void Update()
    {
		bool hasBall = playerStatus.CheckStatusTyp(PlayerStatus.StatusTyp.HAS_BALL);

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
			ball.Throw (transform.position + transform.forward * 2f, transform.forward * throwForce);
		else
			InvokeRPC ("ThrowBall");
	}

	[RPCAttribute]
	public void DropBall()
	{
		if (Host)
			ball.BallDrop (transform.position + transform.up * -1f);
		else
			InvokeRPC("DropBall");
	}
}
