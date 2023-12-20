using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (DiverInput))]
public class Scanner : MonoBehaviour
{
	[SerializeField] float scanRadius = 0.5f;
	[SerializeField] GameObject scanOverlay;

	DiverInput diverInput;
	ScannableObject currentTarget;

	private void Start()
	{
		diverInput = GetComponent <DiverInput>();
	}

	private void Update()
	{
		if (currentTarget)
			ScanInProgress();
		else
			ScanForTarget();
	}

	private void ScanInProgress()
	{
		float distance = Vector3.Distance (transform.position, currentTarget.transform.position);
		bool targetOnScreen = IsOnScreen (currentTarget.transform);

		if (diverInput.IsScanPressed && targetOnScreen && distance <= currentTarget.GeneticInformation.ScanRange)
		{
			SetIndicatorPosition (currentTarget.transform.position);
			Debug.Log ("Scan...");
		}
		else
		{
			currentTarget = null;
			Debug.Log ("Scan abort!");
		}
	}

	private void ScanForTarget()
	{
		RaycastHit hitInfo;
		bool overlayVisible = false;
		
		if (Physics.SphereCast (transform.position, scanRadius, transform.forward, out hitInfo))
		{
			ScannableObject scannableObject = hitInfo.transform.GetComponent<ScannableObject>();

			if (scannableObject && hitInfo.distance <= scannableObject.GeneticInformation.ScanRange)
			{
				if (!scanOverlay.activeInHierarchy)
					scanOverlay.SetActive (true);

				overlayVisible = true;
				SetIndicatorPosition (scannableObject.transform.position);

				if (diverInput.IsScanPressed)
					currentTarget = scannableObject;
			}
		}

		if (scanOverlay.activeInHierarchy != overlayVisible)
			scanOverlay.SetActive (overlayVisible);
	}

	private void SetIndicatorPosition (Vector3 worldPos)
	{
		scanOverlay.transform.position = Camera.main.WorldToScreenPoint (worldPos);
	}

	private bool IsOnScreen (Transform target)
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint (target.position);
		bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
		return onScreen;
	}
}

