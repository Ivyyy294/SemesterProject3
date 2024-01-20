using Ivyyy.Network;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;

public class NetworkManagerCallback : MonoBehaviour
{
	[SerializeField] NetworkManagerUi networkManagerUi;
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
	NetworkPackage networkPackage = new NetworkPackage();

	public bool OnAcceptClient (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		int index = playerConfigurationManager.GetNewPlayerIndex (iPAddress);

		bool accepted = index != -1;

		if (accepted)
			playerConfigurationManager.SetConfigurationIpAddress (index, iPAddress);
		
		return accepted;
	}

	public void OnClientConnected (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		int newPlayerIndex = playerConfigurationManager.GetNewPlayerIndex (iPAddress);
		PlayerConfiguration playerConfiguration = playerConfigurationManager.GetPlayerConfigurationForIp (iPAddress);
		byte[] lastData = playerConfiguration.GetSerializedData();

		networkPackage.Clear();
		networkPackage.AddValue (new NetworkPackageValue (newPlayerIndex));
		networkPackage.AddValue (new NetworkPackageValue (playerConfigurationManager.MaxPlayerCount));
		networkPackage.AddValue (new NetworkPackageValue (lastData));

		int byteSend = socket.Send(networkPackage.GetSerializedData());
		Debug.Log("Client data send! Bytes: " + byteSend);

		networkManagerUi.ShowNotification (playerConfiguration.playerName + " joined!");
	}

	public void OnClientDisconnected (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;
		PlayerConfiguration pc = playerConfigurationManager.GetPlayerConfigurationForIp (iPAddress);

		playerConfigurationManager.ResetConfiguration (pc.playerId);

		networkManagerUi.ShowError (pc.playerName + " disconnected!");
	}

	public void OnHostDisconnected (Socket socket)
	{
		networkManagerUi.ShowError ("Lost connection to host!");
		playerConfigurationManager.ResetConfigurations();
		NetworkSceneController.Me.Owner = true;
		NetworkSceneController.Me.LoadScene (0);
	}

	public void OnConnectedToHost (Socket socket)
	{
		Debug.Log ("Waiting for LocalPlayerId...");

		byte[] buffer = new byte[200];
		int byteCount = socket.Receive (buffer);
		byte[] data = new byte [byteCount];
		Buffer.BlockCopy (buffer, 0, data, 0, data.Length);
		networkPackage.DeserializeData (data);

		int LocalPlayerId = networkPackage.Value (0).GetInt32();
		int maxPlayers = networkPackage.Value (1).GetInt32();
		data = networkPackage.Value (2).GetBytes();

		Debug.Log("PlayerConfigData received!");
		Debug.Log ("LocalPlayerId: " + LocalPlayerId);
		Debug.Log("MaxPlayerCount: " + maxPlayers);

		playerConfigurationManager.InitClientConfiguration (LocalPlayerId, data);
	}

	public void OnHostStarted()
	{
		NetworkSceneController.Me.Owner = true;
		NetworkManager.Me.StartHost (23000);

		playerConfigurationManager.InitHostConfiguration();
		
		NetworkSceneController.Me.LoadScene (1);
	}

	//ToDo coroutine
	public void OnClientStarted (string ip_string)
	{
		NetworkSceneController.Me.Owner = false;

		try
		{
			//Cast input to IPAddress
			IPAddress iPAddress = IPAddress.Parse (ip_string);
			NetworkManager.Me.StartClient (iPAddress.ToString() , 23000);
		}
		catch (Exception excp)
		{
			NetworkManager.Me.ShutDown();
			Debug.LogError ("Can't reach server: " + ip_string);
			return;
		}
	}

	public void ResetNetworkObjects()
	{
		NetworkManager.Me.ShutDown();
		NetworkSceneController.Me.Owner = false;
		PlayerConfigurationManager.Me.ResetConfigurations();
	}

	// Start is called before the first frame update
    void Start()
    {
        playerConfigurationManager = PlayerConfigurationManager.Me;
		networkManager = NetworkManager.Me;
		
		if (networkManager)
		{
			networkManager.acceptClient = OnAcceptClient;
			networkManager.onConnectedToHost = OnConnectedToHost;
			networkManager.onClientConnected = OnClientConnected;
			networkManager.onClientDisonnected = OnClientDisconnected;
			networkManager.onHostDisonnected = OnHostDisconnected;
		}
    }
}
