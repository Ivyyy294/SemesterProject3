using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerMovementProfil normalMovementProfil;
	[SerializeField] PlayerMovementProfil dashMovementProfil;
	[SerializeField] PlayerMovementProfil lowOxygenMovementProfil;
	[SerializeField] PlayerMovementProfil blockMovementProfil;

	public bool IsDashing => currentMovementProfil == dashMovementProfil; 
	
	//Private values
	PlayerOxygen playerOxygen;
	PlayerMovementProfil currentMovementProfil;
    PlayerInput playerInput;
	PlayerBallStatus playerBallStatus;
	PlayerBlock playerBlock;

	Transform targetTransform;
	Rigidbody m_rigidbody;	
	float refSpeed = 0f;
	float currentSpeed = 0f;

	public void ResetRefSpeed()
	{
		refSpeed = 0f;
		currentSpeed = 0f;
	}

    private void Awake()
    {
		InitTargetTransform();

        playerInput = GetComponent<PlayerInput>();
		playerOxygen = targetTransform.GetComponentInChildren<PlayerOxygen>();
		m_rigidbody = targetTransform.GetComponent<Rigidbody>();
		playerBallStatus = targetTransform.GetComponentInChildren<PlayerBallStatus>();
		playerBlock = targetTransform.GetComponentInChildren<PlayerBlock>();

		currentMovementProfil = normalMovementProfil;
    }

    void Update()
    {
		SetCurrentMovementProfile();
	}

	private void FixedUpdate()
	{
		//Block player movement while match is paused
		if (MatchController.Me != null && MatchController.Me.MatchPauseController.IsMatchPaused)
			return;

		Rotation();
		ForwardMovement();
	}

	void ForwardMovement()
	{
		float targetSpeed = currentMovementProfil.maxSpeed * (playerInput.ForwardPressed ? 1f : 0f);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref refSpeed, currentMovementProfil.movementSmoothTime);
		Vector3 newPos = targetTransform.position + (targetTransform.forward * currentSpeed * Time.fixedDeltaTime);
		//m_rigidbody.velocity = Vector3.zero;
		m_rigidbody.MovePosition (newPos);
	}

	void Rotation()
	{
		float turnSpeedDegrees = currentMovementProfil.turnSpeedDegrees;

		Quaternion deltaRotationPitch = Quaternion.Euler(Vector3.right * Time.fixedDeltaTime * turnSpeedDegrees * playerInput.Pitch);
		Quaternion deltaRotationYaw = Quaternion.Euler(Vector3.up * Time.fixedDeltaTime * turnSpeedDegrees * playerInput.Yaw);

		m_rigidbody.MoveRotation (m_rigidbody.rotation * deltaRotationPitch * deltaRotationYaw);

		// how horizontal the diver is, is needed to ONLY auto-correct when the diver a little horizontal
		float levelness = 1 - Mathf.Abs(Vector3.Dot(targetTransform.forward, Vector3.up));
		// how vertical the diver's local RIGHT axis is, is needed to allow loopings
		float twist = Mathf.Abs(Vector3.Dot(targetTransform.right, Vector3.up));

		if (levelness > 0.2f && twist > 0.1f)
		{
			// gradually roll the diver until their local RIGHT axis is horizontal again (their hip-line is horizontal)
			var relativeRotation = Quaternion.FromToRotation(targetTransform.right, GetIdealRightVector());
			var targetRotation = relativeRotation * m_rigidbody.rotation;

			m_rigidbody.MoveRotation (Quaternion.RotateTowards(m_rigidbody.rotation, targetRotation, Time.fixedDeltaTime * turnSpeedDegrees * 0.5f));
		}
	}

	Vector3 GetIdealRightVector()
    {
        return Vector3.Cross(Vector3.up, targetTransform.forward).normalized;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {   
		if (!targetTransform)
			InitTargetTransform();

        // visualize the target orientation for the hips of the diver
        Gizmos.DrawLine(targetTransform.position, targetTransform.position + GetIdealRightVector());
    }
    #endif

	void InitTargetTransform()
	{
		targetTransform = transform.parent ? transform.parent : transform;
	}

	void SetCurrentMovementProfile()
	{
		if (playerOxygen.OxygenEmpty)
			currentMovementProfil = lowOxygenMovementProfil;
		else if (playerBlock.IsBlocking)
			currentMovementProfil = blockMovementProfil;
		//Allow Dash when player has not the ball
		else if (playerInput.DashPressed && !playerBallStatus.HasBall())
			currentMovementProfil = dashMovementProfil;
		else
			currentMovementProfil = normalMovementProfil;
	}
}
