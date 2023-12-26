using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net;

[System.Serializable]
public class PlayerConfiguration :NetworkBehaviour
{
	public string playerName;
	public bool ready = false;
	public bool sceneLoaded = false;
	public IPAddress iPAddress;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (ready));
		networkPackage.AddValue (new NetworkPackageValue (sceneLoaded));
	}

	private void Update()
	{
		if (!Owner && networkPackage.Count > 0)
		{
			ready = networkPackage.Value(0).GetBool();
			sceneLoaded = networkPackage.Value(1).GetBool();
		}
	}
}
