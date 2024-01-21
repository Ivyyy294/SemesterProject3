using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;

	public void Start()
	{
		networkManagerCallback.ResetNetworkObjects();
	}

	public void OnHostPressed()
	{
		networkManagerCallback.OnHostStarted();
	}

	public void OnJoinPressed()
	{
		NetworkManagerHostSessionExplorer searchHostSession = new NetworkManagerHostSessionExplorer();
		searchHostSession.StartSearchHostSession();
			
		while (searchHostSession.HostSessionList.Count <= 0)
			;
		searchHostSession.ShutDownSearchHostSession();

		string ip_string = searchHostSession.HostSessionList[0].ip;

		networkManagerCallback.OnClientStarted (ip_string);
	}
}
