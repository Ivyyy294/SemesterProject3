using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;
using System;
using System.Net;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] TMP_InputField ip;
	[SerializeField] TMP_InputField port;

	public void Start()
	{
		NetworkManager.Me.ShutDown();
		NetworkSceneController.Me.Owner = false;
	}

	public void OnHostPressed()
	{
		NetworkSceneController.Me.Owner = true;
		NetworkManager.Me.StartHost (23000);
		NetworkSceneController.Me.LoadScene (1);
	}

	public void OnJoinPressed()
	{
		NetworkSceneController.Me.Owner = false;
		string ip_string = ip.text;

		IPAddress iPAddress = null;
		try
		{
			//Cast input to IPAddress
			iPAddress = IPAddress.Parse (ip_string);

			NetworkManager.Me.StartClient (iPAddress.ToString() , 23000);
		}
		catch (Exception excp)
		{
			NetworkManager.Me.ShutDown();
			Debug.LogError ("Can't reach server: " + ip);
			return;
		}

	}
}
