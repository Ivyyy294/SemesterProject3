using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class ScannableObject : NetworkBehaviour
{
	int scannerCount;

	protected override void SetPackageData()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Scanner"))
			scannerCount++;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Scanner"))
			scannerCount--;
	}
}
