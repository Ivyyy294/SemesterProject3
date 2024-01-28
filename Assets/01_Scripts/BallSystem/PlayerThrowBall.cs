using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Text;
using UnityEngine.Events;

public class PlayerThrowBall : NetworkBehaviour
{
	[SerializeField] float throwForce = 10f;
	[SerializeField] Transform ballSpawn;

	[HideInInspector] public UnityEvent onBallThrow;

	[Header ("Lara Values")]
	[SerializeField] float initalCooldown = 0.1f;
	PlayerOxygen playerOxygen;
	PlayerBallStatus playerBallStatus;
	PlayerInput playerInput;
	PlayerAudio playerAudio;

	//Private
	Ball ball;
	float timer;

    // Start is called before the first frame update
    void Start()
    {
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();
		playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
		playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
        ball = Ball.Me;

		Owner = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().IsLocalPlayer();
    }

    // Update is called once per frame
    void Update()
    {
		if (MatchController.Me != null && MatchController.Me.MatchPauseController.IsMatchPaused)
			return;

		bool hasBall = playerBallStatus.HasBall();

		//Prevent ball from being thrown instantly
		if (!hasBall)
			timer = 0f;
		else if (timer < initalCooldown)
			timer += Time.deltaTime;

        if (Owner && hasBall && timer > initalCooldown)
		{
			if (playerOxygen.OxygenEmpty)
				DropBall ();
			else if (playerInput.ThrowPressed)
				ThrowBall ();
		}
    }

	protected override void SetPackageData()
	{
	}

	[RPCAttribute]
	public void ThrowBall()
	{
		if (Host)
		{
			ball.Throw (ballSpawn.position, transform.forward * throwForce);
			playerAudio.PlayAudioThrow();
		}
		else
			InvokeRPC ("ThrowBall");

		onBallThrow.Invoke();
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
