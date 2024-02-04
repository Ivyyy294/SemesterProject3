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
	[SerializeField] float safetyDistanz = 0.5f;
	PlayerOxygen playerOxygen;
	PlayerBallStatus playerBallStatus;
	PlayerInputProcessing playerInput;
	PlayerAudio playerAudio;

	//Private
	Ball ball;
	float timer;

    // Start is called before the first frame update
    void Start()
    {
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();

		playerInput = transform.parent.GetComponentInChildren<PlayerInputProcessing>();
		playerInput.OnThrowPressed = OnThrowPressed;

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

		//Drop ball if oxygen empty
        if (Owner && hasBall && playerOxygen.OxygenEmpty)
			DropBall ();
    }

	public void OnThrowPressed()
	{ 
		if (Owner && playerBallStatus.HasBall() && timer > initalCooldown && IsBallBackInPosition())
			ThrowBall ();
	}

	protected override void SetPackageData()
	{
	}

	[RPCAttribute]
	public void ThrowBall()
	{
		if (Host && playerBallStatus.HasBall())
		{
			ball.Throw (ballSpawn.position, transform.forward * throwForce);
		}
		else
			InvokeRPC ("ThrowBall");

		if (Owner)
			playerAudio.PlayAudioThrow();

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

	//Makes sure thrat crawly returned roughly to spawn point before allow a throw
	bool IsBallBackInPosition()
	{
		float distanz = Vector3.Distance (ball.transform.position, ballSpawn.position);
		return distanz < safetyDistanz;
	}
}
