using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerStealBall : NetworkBehaviour
{
	[Range (0.1f, 1f)]
	[SerializeField] float stealRange = 0.5f;
	[SerializeField] float coolDown = 0.1f;
	[SerializeField] LayerMask mask = -1;
	[SerializeField] Transform checkPosition;

	PlayerBallStatus ballStatus;
	PlayerInput playerInput;
	Ball ball;
	short playerId;
	float timer = 0f;

	protected override void SetPackageData() { }

	[RPCAttribute]
	protected void StealBall()
	{
		//Execute Steal only on host session
		if (Host)
		{
			if (ballStatus.HasBall())
				return;

			//Layer ball collider
			Collider[] colliders = Physics.OverlapSphere (checkPosition.position, stealRange, mask.value);

			foreach (Collider i in colliders)
			{
				PlayerCollision playerCollision = i.GetComponentInParent <PlayerCollision>();

				if (playerCollision && playerCollision.PlayerBallStatus.HasBall())
					ball.StealBall (playerId);
			}
		}
		else if (!Host)
			InvokeRPC("StealBall");
	}

    // Start is called before the first frame update
    void Start()
    {
        ballStatus = GetComponent <PlayerBallStatus>();
		playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
		playerId = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>().PlayerID;
		ball = Ball.Me;

		Owner = PlayerConfigurationManager.LocalPlayerId == playerId;
    }

    // Update is called once per frame
    void Update()
    {
        if (Owner && !ballStatus.HasBall()
			&& playerInput.StealPressed
			&& timer >= coolDown)
			StealBall();
		
		if (timer < coolDown)
			timer += Time.deltaTime;
    }

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (checkPosition.position, stealRange);
	}
}
