using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
	[SerializeField] private Gauge colliderInflateGauge;
	[SerializeField] GameObject playerBlockCollider;

	[Header ("Physic Settings")]
	[SerializeField] float blockMass = 1f;
	[SerializeField] float blockDrag = 1f;

	PlayerInputProcessing playerInput;
	PlayerBallStatus playerBallStatus;
	PlayerOxygen playerOxygen;
	Rigidbody mrigidbody;
	Vector3 originalScale;

	float originalMass;
	float originalDrag;

	public bool IsBlocking => playerBlockCollider.activeInHierarchy;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = transform.parent.GetComponentInChildren<PlayerInputProcessing>();
		playerBallStatus = transform.parent.GetComponentInChildren<PlayerBallStatus>();
		playerOxygen = transform.parent.GetComponentInChildren <PlayerOxygen>();

		//Init targetscale with default playerBlockTrigger scale
		originalScale = playerBlockCollider.transform.localScale;

		playerBlockCollider.SetActive (false);

		mrigidbody = GetComponentInParent<Rigidbody>();
		originalMass = mrigidbody.mass;
		originalDrag = mrigidbody.drag;
    }

    // Update is called once per frame
    void Update()
    {
		//Allow block when player dont has ball and oxygen is not empty
		bool block = playerInput.BlockPressed && !playerBallStatus.HasBall() && !playerOxygen.OxygenEmpty;
		
		if (block)
			colliderInflateGauge.Fill();
		else
			colliderInflateGauge.Deplete();

		ScaleBlockCollider (block);
		ApplyPhysicSettings (block);
    }

	void ApplyPhysicSettings (bool block)
	{
		if (block)
		{
			mrigidbody.mass = blockMass;
			mrigidbody.drag = blockDrag;
		}
		else if (mrigidbody.mass == blockMass)
		{
			mrigidbody.mass = originalMass;
			mrigidbody.drag = originalDrag;
		}
	}

	void ScaleBlockCollider (bool block)
	{
		Vector3 currentScale = playerBlockCollider.transform.localScale;
		Vector3 targetScale = colliderInflateGauge.FillAmount * originalScale;
		bool colliderActive = playerBlockCollider.activeInHierarchy;

		if (!colliderActive && block)
			playerBlockCollider.SetActive (true);
		else if (colliderActive && !block && currentScale == Vector3.zero)
			playerBlockCollider.SetActive (false);
		else
			playerBlockCollider.transform.localScale = targetScale;
	}
}
