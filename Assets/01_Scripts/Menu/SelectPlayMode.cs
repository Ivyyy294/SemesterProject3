using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;
using UnityEngine.SceneManagement;
using System;
using System.Net;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] TMP_InputField ip;
	[SerializeField] TMP_InputField port;

	public void OnHostPressed()
	{
		NetworkManager.Me.StartHost (23000);
		SceneManager.LoadScene (1);
	}

	public void OnJoinPressed()
	{
		string ip_string = ip.text;

		IPAddress iPAddress = null;
		try
		{
			//Cast input to IPAddress
			iPAddress = IPAddress.Parse (ip_string);

			if (NetworkManager.Me.StartClient (iPAddress.ToString() , 23000))
				SceneManager.LoadScene (1);
		}
		catch (Exception excp)
		{
			Debug.LogError ("Can't reach server: " + ip);
			return;
		}

	}
}
