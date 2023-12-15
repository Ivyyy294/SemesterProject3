using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net.Sockets;
using System;

public class PlayerConfigurationManager : MonoBehaviour
{
	public int LocalPlayerId { get; set;}
	static public PlayerConfigurationManager Me {get; private set;}
	public PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];

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

	private void Update()
	{
		LocalPlayerId = NetworkManager.Me.Host ? 0 : 1;
	}
}
