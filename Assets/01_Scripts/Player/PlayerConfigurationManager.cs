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
	
	[Header ("Lara values")]
	public PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];

	//Private Values

	//Public Methods
	//ToDo Remove
	public static int LocalPlayerId { get; set;}	
	public static int LocalPlayerTeamIndex => (Me ? Me.playerConfigurations[LocalPlayerId].teamNr : 0);
	public int MaxPlayerCount { get; set;}
	public static string LocalPlayerName => (Me ? Me.playerConfigurations[LocalPlayerId].playerName : "Local tester");

	public bool PlayersReady()
	{
		for (int i = 0; i < MaxPlayerCount; ++i)
		{
			if (!playerConfigurations[i].ready)
				return false;
		}

		return true;
	}

	public bool EqualTeamSize()
	{
		int sizeTeam1 = 0;
		int sizeTeam2 = 0;

		for (int i = 0; i < MaxPlayerCount; ++i)
		{
			if (playerConfigurations[i].teamNr == 0)
				sizeTeam1++;
			else
				sizeTeam2++;
		}

		return sizeTeam1 == sizeTeam2;
	}

	public void ResetPlayers()
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
		{
			playerConfigurations[i].ready = false;
		}
	}

	public void ResetConfigurations()
	{
		for (int i = 0; i < playerConfigurations.Length; ++i)
			playerConfigurations[i].ResetConfiguration(GetDefaultName (i));
	}

	public void SoftResetConfiguration (int index)
	{
		if (index < playerConfigurations.Length)
			playerConfigurations[index].SoftResetConfiguration();
		else
			Debug.LogError("Invalid playerConfigurations index!");
	}

	public int GetNewPlayerIndex(IPAddress iPAddress)
	{
		//Player 0 is always host
		for (int i = 1; i < playerConfigurations.Length && i < MaxPlayerCount; ++i)
		{
			PlayerConfiguration playerConfiguration = playerConfigurations[i];

			//Returning player or New player
			if (playerConfiguration.iPAddress == null
				||  (!playerConfiguration.connected && playerConfiguration.iPAddress.Equals(iPAddress)))
				return i;
		}

		return -1;
	}

	public void SetConfigurationConnectionData (int index, IPEndPoint iPEndPoint)
	{
		if (index < playerConfigurations.Length)
		{
			playerConfigurations[index].SetConnectionData (iPEndPoint);
			playerConfigurations[index].connected = true;
		}
		else
			Debug.LogError ("Invalid playerConfigurations index!");
	}

	public PlayerConfiguration GetConfigurationForIpEndpoint (IPEndPoint iPEndPoint)
	{
		foreach (PlayerConfiguration i in playerConfigurations)
		{
			if (i.iPAddress != null && (i.iPAddress.Equals(iPEndPoint.Address))
				&& i.port == iPEndPoint.Port)
				return i;
		}

		return null;
	}

	public void InitClientConfiguration (int index, byte[] data)
	{
		LocalPlayerId = index;
		playerConfigurations[LocalPlayerId].DeserializeData (data);
		playerConfigurations[LocalPlayerId].ReadPackageData();
		playerConfigurations[LocalPlayerId].Owner = true;
		playerConfigurations[LocalPlayerId].connected = true;
	}

	public void InitHostConfiguration ()
	{
		playerConfigurations[0].Owner = true;
		playerConfigurations[0].connected = true;
		LocalPlayerId = 0;
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
			Destroy (gameObject);
	}

	private void Start()
	{
		LocalPlayerId = 0;

		for (short i = 0; i < playerConfigurations.Length; ++i)
			playerConfigurations[i].playerId = i;
	}

	private string GetDefaultName (int index)
	{
		string defaultName = "KingCrab";

		if (index == 1)
			defaultName = "Lobster";
		else if (index == 2)
			defaultName = "RainbowTrout";
		else if (index == 2)
			defaultName = "ClownFish";

		return defaultName;
	}
}
