using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class ScannableObject : NetworkBehaviour
{
	[Min (0.1f)]
	[SerializeField] float targetScanTime;
	int scannerCount = 0;
	bool scanned = false;
	float scanTimer = 0f;
	GeneticInformation geneticInformation;

	//Public
	public bool IsScanned { get { return scanned;} }

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (scanned));
	}

	//Private
	private void Start()
	{
		if (NetworkManager.Me)
			Owner = NetworkManager.Me.Host;
		else
			Owner = true;

		geneticInformation = GetComponent <GeneticInformation>();
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
			
			scanned = scanTimer > targetScanTime;
		}
		else if (networkPackage.Count > 0)
			scanned = networkPackage.Value(0).GetBool();
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
