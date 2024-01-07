using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net.Sockets;
using System;
using System.Net;

public class PlayerConfigurationManager : MonoBehaviour
{
	//Public Values
	static public PlayerConfigurationManager Me {get; private set;}
	public PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];

	//Private Values
	[SerializeField] int maxPlayers = 4;
	NetworkManager networkManager;

	//Public Methods
	//ToDo Remove
	public int LocalPlayerId { get; set;}
	public int MaxPlayerCount { get { return maxPlayers;} }

	public bool IsClientConnected (int indexPlayer)
	{
		if (indexPlayer < playerConfigurations.Length)
		{
			//Player 0 is always host
			if (indexPlayer == LocalPlayerId)
				return true;
			else
				return playerConfigurations[indexPlayer].connected;
		}
		else
			return false;
	}

	public bool PlayersReady()
	{
		for (int i = 0; i < MaxPlayerCount; ++i)
		{
			if (!playerConfigurations[i].ready)
				return false;
		}

		return true;
	}

	public bool PlayersLoadedScene()
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
		{
			if (!playerConfigurations[i].sceneLoaded)
				return false;
		}

		return true;
	}

	public void ResetPlayers()
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
		{
			playerConfigurations[i].ready = false;
		}
	}

	//NetworkManagerCallbacks
	public bool OnAcceptClient (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		return GetNewPlayerIndex (iPAddress) != -1;
	}

	public void OnConnectedToHost (Socket socket)
	{
		Debug.Log ("Waiting for LocalPlayerId...");
		byte[] buffer = new byte[sizeof(int)];
		int tmpId = -1;

		socket.Receive (buffer);
		tmpId = BitConverter.ToInt32 (buffer, 0);

		if (tmpId != -1)
		{
			LocalPlayerId = tmpId;
			Debug.Log ("LocalPlayerId: " + LocalPlayerId);
			playerConfigurations[LocalPlayerId].Owner = true;
			playerConfigurations[LocalPlayerId].connected = true;
		}
		else
			Debug.Log("Time out getting LocalPlayerId!");

		//GetMaxPlayer
		socket.Receive(buffer);
		maxPlayers = BitConverter.ToInt32(buffer, 0);
		Debug.Log("MaxPlayerCount: " + maxPlayers);

	}

	public void OnClientConnected (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		int newPlayerIndex = GetNewPlayerIndex (iPAddress);
		Debug.Log ("ClientId: " + newPlayerIndex);

		int byteSend = socket.Send(BitConverter.GetBytes (newPlayerIndex));
		Debug.Log (socket.RemoteEndPoint);
		playerConfigurations[newPlayerIndex].iPAddress = iPAddress;
		Debug.Log ("ClientId send!: " + byteSend);

		//Send max player count to player
		socket.Send(BitConverter.GetBytes(maxPlayers));
		Debug.Log("Max Players send!");
	}

	//Private Methods
	private void Awake()
	{
		if (Me == null)
		{
			Me = this;
			DontDestroyOnLoad (this);
		}
		else
			Destroy (this);
	}

	private void Start()
	{
		networkManager = NetworkManager.Me;
		LocalPlayerId = 0;

		if (networkManager)
		{
			networkManager.acceptClient = OnAcceptClient;
			networkManager.onConnectedToHost = OnConnectedToHost;
			networkManager.onClientConnected = OnClientConnected;
		}
	}

	private void Update()
	{
		if (networkManager.Host && !playerConfigurations[0].Owner)
		{
			//Make sure Host in owner of their confuguration
			playerConfigurations[0].Owner = true;
			playerConfigurations[0].connected = true;
		}
	}

	int GetNewPlayerIndex(IPAddress iPAddress)
	{
		//Player 0 is always host
		for (int i = 1; i < playerConfigurations.Length && i < maxPlayers; ++i)
		{
			//New player
			if (playerConfigurations[i].iPAddress == null)
				return i;
			////Returning player
			//else if (playerConfigurations[i].iPAddress.Equals (iPAddress))
			//	return i;
		}

		return -1;
	}
}
