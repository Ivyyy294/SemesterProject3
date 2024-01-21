#define LOCAL_DEBUG

using Ivyyy.Network;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class NetworkManagerCallback : MonoBehaviour
{
	[SerializeField] NetworkManagerUi networkManagerUi;
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
	NetworkPackage networkPackage = new NetworkPackage();

	NetworkManagerHostSessionBroadcast hostSessionBroadcast;

	public bool OnAcceptClient (Socket socket)
	{
		IPEndPoint iPEndPoint = (IPEndPoint) socket.RemoteEndPoint;

		int index = playerConfigurationManager.GetNewPlayerIndex (iPEndPoint.Address);

		bool accepted = index != -1;
		
		if (accepted)
			playerConfigurationManager.SetConfigurationConnectionData (index, iPEndPoint);

		return accepted;
	}

	public void OnClientConnected (Socket socket)
	{
		IPEndPoint iPEndPoint = (IPEndPoint) socket.RemoteEndPoint;
		PlayerConfiguration playerConfiguration = playerConfigurationManager.GetConfigurationForIpEndpoint (iPEndPoint);

		byte[] lastData = playerConfiguration.GetSerializedData();

		networkPackage.Clear();
		networkPackage.AddValue (new NetworkPackageValue (playerConfiguration.playerId));
		networkPackage.AddValue (new NetworkPackageValue (playerConfigurationManager.MaxPlayerCount));
		networkPackage.AddValue (new NetworkPackageValue (lastData));

		int byteSend = socket.Send(networkPackage.GetSerializedData());
		Debug.Log("Client data send! Bytes: " + byteSend);

		networkManagerUi.ShowNotification (playerConfiguration.playerName + " joined!");
	}

	public void OnClientDisconnected (Socket socket)
	{
		IPEndPoint iPEndPoint = (IPEndPoint) socket.RemoteEndPoint;
		PlayerConfiguration playerConfiguration = playerConfigurationManager.GetConfigurationForIpEndpoint (iPEndPoint);

		playerConfigurationManager.SoftResetConfiguration (playerConfiguration.playerId);

		networkManagerUi.ShowError (playerConfiguration.playerName + " disconnected!");
	}

	public void OnHostDisconnected (Socket socket)
	{
		networkManagerUi.ShowError ("Lost connection to host!");
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

		int LocalPlayerId = networkPackage.Value (0).GetShort();
		int maxPlayers = networkPackage.Value (1).GetInt32();
		data = networkPackage.Value (2).GetBytes();

		Debug.Log("PlayerConfigData received!");
		Debug.Log ("LocalPlayerId: " + LocalPlayerId);
		Debug.Log("MaxPlayerCount: " + maxPlayers);

		playerConfigurationManager.InitClientConfiguration (LocalPlayerId, data);
	}

	public void OnHostStarted()
	{
		PlayerConfigurationManager.Me.ResetConfigurations();
		NetworkSceneController.Me.Owner = true;
		NetworkManager.Me.StartHost (23000);

		hostSessionBroadcast = new NetworkManagerHostSessionBroadcast();
		hostSessionBroadcast.StartHostSessionBroadcast ("ScubaFun");

		if (!playerConfigurationManager)
			playerConfigurationManager = PlayerConfigurationManager.Me;

		playerConfigurationManager.InitHostConfiguration();
		
		NetworkSceneController.Me.LoadScene (1);
	}

	//ToDo coroutine
	public void OnClientStarted (string ip_string)
	{
		NetworkSceneController.Me.Owner = false;
		PlayerConfigurationManager.Me.ResetConfigurations();

		Task.Run(()=>
		{
			try
			{
				//Cast input to IPAddress
				IPAddress iPAddress = IPAddress.Parse (ip_string);
				bool ok = NetworkManager.Me.StartClient (iPAddress.ToString() , 23000);
			}
			catch (Exception excp)
			{
				NetworkManager.Me.ShutDown();
				networkManagerUi.ShowError ("Can't reach server: " + ip_string);
				return;
			}
		});
	}

	public void ResetNetworkObjects()
	{
		if (hostSessionBroadcast != null)
		{
			hostSessionBroadcast.ShutDownHostSessionBroadcast();
			hostSessionBroadcast = null;
		}

		NetworkManager.Me.ShutDown();
		NetworkSceneController.Me.Owner = false;
		PlayerConfigurationManager.Me.ResetConfigurations();
	}

	//Private Methods
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

	private void OnDestroy()
	{
		ResetNetworkObjects();
	}
}
