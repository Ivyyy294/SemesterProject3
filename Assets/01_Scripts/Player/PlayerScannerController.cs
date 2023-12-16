using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerScannerController : NetworkBehaviour
{
	[SerializeField] Scanner playerScanner;
	[SerializeField] KeyCode scanKey;
	bool scannerActive = false;
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (scannerActive));
	}

    // Update is called once per frame
    void Update()
    {
		if (Owner)
		{
			if (Input.GetKeyDown (scanKey))
				scannerActive = true;
			else if (Input.GetKeyUp(scanKey))
				scannerActive = false;
		}
		else if (networkPackage.Count > 0)
			scannerActive = networkPackage.Value (0).GetBool();

		if (playerScanner)
			playerScanner.SetActivate (scannerActive);
    }

	//[RPCAttribute]
	//void ActivateScanner()
	//{
	//	if (Owner)
	//		Invoke ("ActivateScanner");

	//	if (playerScanner)
	//		playerScanner.SetActivate (true);
	//}

	//[RPCAttribute]
	//void DeactivateScanner()
	//{
	//	if (Owner)
	//		Invoke ("DeactivateScanner");

	//	if (playerScanner)
	//		playerScanner.SetActivate (false);
	//}

}
