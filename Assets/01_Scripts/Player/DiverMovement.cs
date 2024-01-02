using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DiverInput), typeof (Rigidbody))]
public class DiverMovement : MonoBehaviour
{
	[Range (0f, 5f)]
    [SerializeField] private float movementSpeed = 1f;

    [SerializeField] private float turnSpeedDegrees = 10f;

	[Range (0.3f, 1f)]
	[SerializeField] private float movementSmoothTime = 0.3f;

	//Private values
	float refSpeed = 0f;
	float currentSpeed = 0f;
	Rigidbody m_rigidbody;

    private DiverInput diverInput;

    private void Awake()
    {
        diverInput = GetComponent<DiverInput>();
		m_rigidbody = GetComponent <Rigidbody>();
    }

    void Update()
    {
		transform.Rotate(Vector3.right, Time.deltaTime * turnSpeedDegrees * diverInput.Pitch);
		transform.Rotate(Vector3.up, Time.deltaTime * turnSpeedDegrees * diverInput.Yaw);

		// how horizontal the diver is, is needed to ONLY auto-correct when the diver a little horizontal
		float levelness = 1 - Mathf.Abs(Vector3.Dot(transform.forward, Vector3.up));
		// how vertical the diver's local RIGHT axis is, is needed to allow loopings
		float twist = Mathf.Abs(Vector3.Dot(transform.right, Vector3.up));

		if (levelness > 0.2f && twist > 0.1f)
		{
			// gradually roll the diver until their local RIGHT axis is horizontal again (their hip-line is horizontal)
			var relativeRotation = Quaternion.FromToRotation(transform.right, GetIdealRightVector());
			var targetRotation = relativeRotation * transform.rotation;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * turnSpeedDegrees * 0.5f);
		}
	}

	private void FixedUpdate()
	{
		ForwardMovement();
	}

	void ForwardMovement()
	{
		float targetSpeed = movementSpeed * (diverInput.ForwardPressed ? 1f : 0f);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref refSpeed, movementSmoothTime);
		Vector3 newPos = transform.position + (transform.forward * currentSpeed * Time.fixedDeltaTime);
		m_rigidbody.MovePosition (newPos);
	}

	Vector3 GetIdealRightVector()
    {
        return Vector3.Cross(Vector3.up, transform.forward).normalized;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {   
        // visualize the target orientation for the hips of the diver
        Gizmos.DrawLine(transform.position, transform.position + GetIdealRightVector());
    }
    #endif
}
