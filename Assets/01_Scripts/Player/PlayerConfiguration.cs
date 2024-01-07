using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net;

[System.Serializable]
public class PlayerConfiguration :NetworkBehaviour
{
	public string playerName = "";
	public bool ready = false;
	public bool sceneLoaded = false;
	public bool connected = false;
	public IPAddress iPAddress = null;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (ready));			//0
		networkPackage.AddValue (new NetworkPackageValue (sceneLoaded));	//1
		networkPackage.AddValue (new NetworkPackageValue (connected));		//2
		networkPackage.AddValue (new NetworkPackageValue (playerName));		//3
	}

	private void Update()
	{
		if (!Owner && networkPackage.Available)
		{
			ready = networkPackage.Value(0).GetBool();
			sceneLoaded = networkPackage.Value(1).GetBool();
			connected = networkPackage.Value(2).GetBool();

			if (networkPackage.Count > 3)
				playerName = networkPackage.Value(3).GetString();
		}
	}
}
