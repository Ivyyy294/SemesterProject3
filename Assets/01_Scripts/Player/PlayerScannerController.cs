using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerScannerController : NetworkBehaviour
{
	[SerializeField] Scanner playerScanner;
	[SerializeField] KeyCode scanKey;

	protected override void SetPackageData(){}

    // Update is called once per frame
    void Update()
    {
		if (Owner)
		{
			if (Input.GetKeyDown (scanKey))
				ActivateScanner();
			else if (Input.GetKeyUp(scanKey))
				DeactivateScanner();
		}
    }

	[RPCAttribute]
	void ActivateScanner()
	{
		if (Owner)
			Invoke ("ActivateScanner");

		if (playerScanner)
			playerScanner.SetActivate (true);
	}

	[RPCAttribute]
	void DeactivateScanner()
	{
		if (Owner)
			Invoke ("DeactivateScanner");

		if (playerScanner)
			playerScanner.SetActivate (false);
	}

}
