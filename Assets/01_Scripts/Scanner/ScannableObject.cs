using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class ScannableObject : NetworkBehaviour
{
	[Min (0.1f)]
	[SerializeField] float targetScanTime;
	int scannerCount = 0;
	bool active = true;
	float scanTimer = 0f;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (active));
	}

	private void Start()
	{
		if (NetworkManager.Me)
			Owner = NetworkManager.Me.Host;
		else
			Owner = true;
	}

	private void Update()
	{
		if (Owner)
		{
			if (scannerCount > 0)
				scanTimer += Time.deltaTime * scannerCount;
			//Reset scanTimer if no player has lock
			else
				scanTimer = 0f;
			
			active = scanTimer < targetScanTime;
		}
		else if (networkPackage.Count > 0)
			active = networkPackage.Value(0).GetBool();

		gameObject.SetActive (active);
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Scanner"))
			scannerCount++;
	}

	private void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Scanner"))
			scannerCount--;
	}
}
