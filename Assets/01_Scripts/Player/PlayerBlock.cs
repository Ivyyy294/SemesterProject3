using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
	[SerializeField] GameObject playerBlockCollider;
	[SerializeField] GameObject playerBlockTrigger;

	PlayerInput playerInput;
	PlayerBallStatus playerBallStatus;
	PlayerOxygen playerOxygen;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
		//Allow block when player dont has ball and oxygen is not empty
		bool block = playerInput.BlockPressed && !playerBallStatus.HasBall() && !playerOxygen.OxygenEmpty;

		if (playerBlockCollider.activeInHierarchy != block)
		{
			playerBlockCollider.SetActive (block);
			playerBlockTrigger.SetActive (block);
		}
    }
}
