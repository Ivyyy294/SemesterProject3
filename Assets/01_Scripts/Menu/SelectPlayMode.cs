using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] TMP_InputField ip;
	[SerializeField] TMP_InputField port;
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
		networkManagerCallback.OnClientStarted (ip.text);
	}

	
}
