using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net.Sockets;
using System;
using System.Net;

public class PlayerConfigurationManager : MonoBehaviour
{
	public int LocalPlayerId { get; set;}
	static public PlayerConfigurationManager Me {get; private set;}
	public PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];
	public bool ClientConnected {get{return playerConfigurations[1].iPAddress != null;} }

	NetworkManager networkManager;

	public bool PlayersReady()
	{
		return playerConfigurations[0].ready && playerConfigurations[1].ready;
	}

	public bool PlayersLoadedScene()
	{
		return playerConfigurations[0].sceneLoaded && playerConfigurations[1].sceneLoaded;
	}

	public void ResetPlayers()
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
		{
			playerConfigurations[1].ready = false;
		}
	}

	//NetworkManagerCallbacks
	public bool OnAcceptClient (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		//New Client
		if (playerConfigurations[1].iPAddress == null)
			return true;
		else if (playerConfigurations[1].iPAddress.Equals (iPAddress))
		{
			Debug.Log ("Client re-joned: " + iPAddress);
			return true;
		}
		else
		{
			Debug.Log ("Client rejected: " + iPAddress);
			return false;
		}
		//Re-joining CLient
	}

	public void OnClientConnected (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;
		playerConfigurations[1].iPAddress = iPAddress;
	}

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

		if (networkManager)
		{
			networkManager.acceptClient = OnAcceptClient;
			networkManager.onClientConnected = OnClientConnected;
		}
	}

	private void Update()
	{
		LocalPlayerId = networkManager.Host ? 0 : 1;
	}
}
