using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (DiverInput))]
public class Scanner : NetworkBehaviour
{
	[SerializeField] float scanRadius = 0.5f;
	[SerializeField] GameObject scanOverlay;

	DiverInput diverInput;
	ScannableObject currentTarget;
	string targetGuid = "";

	//Public
	public string CurrentTargetId { get { return targetGuid;} }

	//Protected Methods
	protected override void SetPackageData()
	{
		if (currentTarget)
			networkPackage.AddValue (new NetworkPackageValue (targetGuid));
	}

	//Private Methods
	private void Start()
	{
		diverInput = GetComponent <DiverInput>();
	}

	private void Update()
	{
		if (Owner)
		{
			if (currentTarget)
				ScanInProgress();
			else
				ScanForTarget();
		}
		else
		{
			if (networkPackage.Available)
				targetGuid = networkPackage.Value(0).GetString();
			else
				targetGuid = "";
		}

		//Disable Scan Overlay for remote player
		if (!Owner && scanOverlay.activeInHierarchy)
			scanOverlay.SetActive (false);
	}

	private void ScanInProgress()
	{
		float distance = Vector3.Distance (transform.position, currentTarget.transform.position);
		bool targetOnScreen = IsOnScreen (currentTarget.transform);

		//Scan Complete
		if (currentTarget.IsScanned())
			SetTarget (null);
		//Scan pending
		else if (diverInput.IsScanPressed && targetOnScreen && distance <= currentTarget.GeneticInformation.ScanRange)
			SetIndicatorPosition (currentTarget.transform.position);
		//Scan abort
		else
		{
			SetTarget (null);
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

			if (scannableObject
				&& !scannableObject.IsScanned()
				&& hitInfo.distance <= scannableObject.GeneticInformation.ScanRange)
			{
				if (!scanOverlay.activeInHierarchy)
					scanOverlay.SetActive (true);

				overlayVisible = true;
				SetIndicatorPosition (scannableObject.transform.position);

				if (diverInput.IsScanPressed)
					SetTarget (scannableObject);
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

	private void SetTarget (ScannableObject target)
	{
		currentTarget = target;
		
		if (target)
			targetGuid = target.GeneticInformation.GUID;
		else
			targetGuid = "";
	}
}

