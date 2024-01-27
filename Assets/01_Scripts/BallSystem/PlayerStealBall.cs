using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerStealBall : MonoBehaviour
{
	[Range (0.1f, 1f)]
	[SerializeField] float stealRange = 0.5f;
	[SerializeField] float stealDuration = 0.25f;
	[SerializeField] LayerMask mask = -1;
	[SerializeField] Transform checkPosition;

	PlayerBallStatus ballStatus;
	PlayerInput playerInput;
	Ball ball;
	PlayerAudio playerAudio;
	short playerId;
	float timer = 0f;
	bool host = false;

	[RPCAttribute]
	protected void StealBall ()
	{
		//Execute Steal only on host session
		if (ballStatus.HasBall())
			return;

		//Layer ball collider
		Collider[] colliders = Physics.OverlapSphere (checkPosition.position, stealRange, mask.value);

		if (colliders.Length > 0)
		{
			foreach (Collider i in colliders)
			{
				PlayerCollision playerCollision = i.GetComponentInParent <PlayerCollision>();

				if (playerCollision && playerCollision.PlayerBallStatus.HasBall())
					timer += Time.deltaTime;
			}
		}
		else
			timer = 0f;

		if (timer >= stealDuration)
		{
			ball.SetPlayerId (playerId);
			playerAudio.PlayAudioSteal();
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        ballStatus = GetComponent <PlayerBallStatus>();
		playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
		playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
		playerId = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>().PlayerID;
		ball = Ball.Me;

		host = !NetworkManager.Me || NetworkManager.Me.Host;
    }

    // Update is called once per frame
    void Update()
    {
        if (host && !ballStatus.HasBall()
			&& playerInput.StealPressed)
			StealBall ();
		
		if (!playerInput.StealPressed && timer > 0f)
			timer = 0f;
    }

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (checkPosition.position, stealRange);
	}
}
