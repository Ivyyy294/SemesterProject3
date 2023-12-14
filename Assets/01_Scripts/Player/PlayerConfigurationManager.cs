using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net.Sockets;
using System;

public class PlayerConfigurationManager : MonoBehaviour
{
	[SerializeField] PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];
	public int LocalPlayerId { get; set;}

	bool spawn = false;

    void Start()
    {
		NetworkManager.Me.onClientConnected = OnClientConnected;
		NetworkManager.Me.onConnectedToHost = OnConnectedToHost;
		EnablePlayerObjects (false);
    }

	private void Update()
	{
		if (spawn)
		{
			EnablePlayerObjects (true);
			spawn = false;

			SetOwnerState (playerConfigurations[0].playerObj, NetworkManager.Me.Host);
			SetOwnerState (playerConfigurations[1].playerObj, !NetworkManager.Me.Host);
		}
	}

	void OnClientConnected(int clientNumber, Socket socket)
	{
		//Send client player index to client
		Debug.Log ("Send Client player id: " + clientNumber);
		socket.Send (BitConverter.GetBytes (clientNumber));
		spawn = true;
	}

	void OnConnectedToHost(Socket socket)
	{
		//Get local player index from host
		byte[] buffer = new byte[sizeof (int)];
		int bysteCount = socket.Receive (buffer);
		LocalPlayerId = BitConverter.ToInt32 (buffer, 0);
		Debug.Log ("LocalPlayerId: " + LocalPlayerId);
		spawn = true;
	}

	private void SetOwnerState (GameObject obj, bool val)
	{
		if (obj)
		{
			NetworkBehaviour[] networkBehaviours = obj.GetComponentsInChildren <NetworkBehaviour>();

			foreach (NetworkBehaviour i in networkBehaviours)
				i.Owner = val;

			if (!val)
			{
				Camera camera = obj.GetComponentInChildren<Camera>();
				camera.gameObject.SetActive (false);
			}
		}
	}

	void EnablePlayerObjects(bool val)
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
		{
			if (playerConfigurations[i].playerObj)
				playerConfigurations[i].playerObj.SetActive (val);
		}
	}
}
