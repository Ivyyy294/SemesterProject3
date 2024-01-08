using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(DiverInput))]
public class DiverMovement : MonoBehaviour
{
	[Range (0f, 5f)]
    [SerializeField] private float movementSpeedNormal = 1f;

	[Range (0f, 5f)]
    [SerializeField] private float movementSpeedOxygenEmpty = 0.5f;

    [SerializeField] private float turnSpeedDegrees = 10f;

	[Range (0.3f, 1f)]
	[SerializeField] private float movementSmoothTime = 0.3f;


	//Private values
	PlayerOxygen playerOxygen;
	Transform targetTransform;
	Rigidbody m_rigidbody;	
	float refSpeed = 0f;
	float currentSpeed = 0f;

    private DiverInput diverInput;

    private void Awake()
    {
        diverInput = GetComponent<DiverInput>();
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		targetTransform = transform.parent;
		m_rigidbody = targetTransform.GetComponent<Rigidbody>();
    }

    void Update()
    {
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
		float targetSpeed = (playerOxygen.OxygenEmpty? movementSpeedOxygenEmpty : movementSpeedNormal) * (diverInput.ForwardPressed ? 1f : 0f);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref refSpeed, movementSmoothTime);
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
        // visualize the target orientation for the hips of the diver
        Gizmos.DrawLine(targetTransform.position, targetTransform.position + GetIdealRightVector());
    }
    #endif
}
