using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
	[SerializeField] float easeStepSize = 1f;
	[SerializeField] GameObject playerBlockCollider;

	PlayerInput playerInput;
	PlayerBallStatus playerBallStatus;
	PlayerOxygen playerOxygen;

	Vector3 originalScale;

	public bool IsBlocking => playerBlockCollider.activeInHierarchy;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();

		//Init targetscale with default playerBlockTrigger scale
		originalScale = playerBlockCollider.transform.localScale;

		playerBlockCollider.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
		//Allow block when player dont has ball and oxygen is not empty
		bool block = playerInput.BlockPressed && !playerBallStatus.HasBall() && !playerOxygen.OxygenEmpty;
		bool colliderActive = playerBlockCollider.activeInHierarchy;

		Vector3 currentScale = playerBlockCollider.transform.localScale;

		Vector3 targetScale = block ? originalScale : Vector3.zero;
		targetScale = Vector3.MoveTowards (playerBlockCollider.transform.localScale, targetScale, easeStepSize * Time.deltaTime);

		if (!colliderActive && block)
			playerBlockCollider.SetActive (true);
		else if (colliderActive && !block && currentScale == Vector3.zero)
			playerBlockCollider.SetActive (false);
		else
			playerBlockCollider.transform.localScale = targetScale;
    }
}
