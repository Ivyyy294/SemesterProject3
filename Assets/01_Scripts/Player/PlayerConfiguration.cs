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
	public bool invertYAxis = false;
	public IPAddress iPAddress = null;
	public int port = 0;
	public int teamNr = 0;
	public bool lgbtq = false;

	//Not synced
	public short playerId = 0;

	public void SetConnectionData (IPEndPoint iPEndPoint)
	{
		iPAddress = iPEndPoint.Address;
		port = iPEndPoint.Port;
	}

	public void ResetConfiguration (string defaultName)
	{
		Owner = false;
		playerName = defaultName;
		ready = false;
		sceneLoaded = false;
		connected = false;
		iPAddress = null;
		port = 0;
		teamNr = 0;
	}

	public void SoftResetConfiguration ()
	{
		Owner = false;
		ready = false;
		sceneLoaded = false;
		connected = false;
		port = 0;
	}

	public void ReadPackageData()
	{
		ready = networkPackage.Value(0).GetBool();
		sceneLoaded = networkPackage.Value(1).GetBool();
		connected = networkPackage.Value(2).GetBool();
		playerName = networkPackage.Value(3).GetString();
		teamNr = networkPackage.Value(4).GetInt32();
		invertYAxis = networkPackage.Value (5).GetBool();
		lgbtq = networkPackage.Value (6).GetBool();
		networkPackage.Clear();
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (ready));			//0
		networkPackage.AddValue (new NetworkPackageValue (sceneLoaded));	//1
		networkPackage.AddValue (new NetworkPackageValue (connected));		//2
		networkPackage.AddValue (new NetworkPackageValue (playerName));		//3
		networkPackage.AddValue (new NetworkPackageValue (teamNr));			//4
		networkPackage.AddValue (new NetworkPackageValue (invertYAxis));	//5
		networkPackage.AddValue (new NetworkPackageValue (lgbtq));	//6
	}

	private void Update()
	{
		if (!Owner && networkPackage.Available)
			ReadPackageData();
	}
}
