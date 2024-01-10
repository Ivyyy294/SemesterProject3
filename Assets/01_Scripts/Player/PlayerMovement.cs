using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(DiverInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerMovementProfil normalMovementProfil;
	[SerializeField] PlayerMovementProfil dashMovementProfil;
	[SerializeField] PlayerMovementProfil lowOxygenMovementProfil;

	//Private values
	PlayerOxygen playerOxygen;
	Transform targetTransform;
	Rigidbody m_rigidbody;	
	float refSpeed = 0f;
	float currentSpeed = 0f;
	PlayerMovementProfil currentMovementProfil;

    private DiverInput diverInput;

    private void Awake()
    {
		InitTargetTransform();
        diverInput = GetComponent<DiverInput>();
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		m_rigidbody = targetTransform.GetComponent<Rigidbody>();
		currentMovementProfil = normalMovementProfil;
    }

    void Update()
    {
		SetCurrentMovementProfile();

		float turnSpeedDegrees = currentMovementProfil.turnSpeedDegrees;

		targetTransform.Rotate(Vector3.right, Time.deltaTime * turnSpeedDegrees * diverInput.Pitch);
		targetTransform.Rotate(Vector3.up, Time.deltaTime * turnSpeedDegrees * diverInput.Yaw);

		// how horizontal the diver is, is needed to ONLY auto-correct when the diver a little horizontal
		float levelness = 1 - Mathf.Abs(Vector3.Dot(targetTransform.forward, Vector3.up));
		// how vertical the diver's local RIGHT axis is, is needed to allow loopings
		float twist = Mathf.Abs(Vector3.Dot(targetTransform.right, Vector3.up));

		if (levelness > 0.2f && twist > 0.1f)
		{
			// gradually roll the diver until their local RIGHT axis is horizontal again (their hip-line is horizontal)
			var relativeRotation = Quaternion.FromToRotation(targetTransform.right, GetIdealRightVector());
			var targetRotation = relativeRotation * targetTransform.rotation;
			targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, targetRotation, Time.deltaTime * turnSpeedDegrees * 0.5f);
		}
	}

	private void FixedUpdate()
	{
		ForwardMovement();
	}

	void ForwardMovement()
	{
		float targetSpeed = currentMovementProfil.maxSpeed * (diverInput.ForwardPressed ? 1f : 0f);
		Debug.Log ("TargetSpeed:" + targetSpeed);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref refSpeed, currentMovementProfil.movementSmoothTime);
		Vector3 newPos = targetTransform.position + (targetTransform.forward * currentSpeed * Time.fixedDeltaTime);
		//m_rigidbody.velocity = Vector3.zero;
		m_rigidbody.MovePosition (newPos);
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
        Gizmos.DrawLine(transform.parent.position, targetTransform.position + GetIdealRightVector());
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
		else if (diverInput.DashPressed)
			currentMovementProfil = dashMovementProfil;
		else
			currentMovementProfil = normalMovementProfil;
	}
}
