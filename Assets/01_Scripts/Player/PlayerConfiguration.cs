using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[System.Serializable]
public class PlayerConfiguration :NetworkBehaviour
{
	public string playerName;
	public bool ready;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (ready));
	}

	private void Update()
	{
		if (!Owner && networkPackage.Count > 0)
			ready = networkPackage.Value(0).GetBool();
	}
}
