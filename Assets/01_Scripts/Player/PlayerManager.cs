using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ivyyy.Network;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] GameObject[] playerList = new GameObject[2];
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
	static GameObject localPlayer;
	
	public static PlayerManager Me { get; private set; }

	public GameObject[] PlayerList => playerList;
	public IEnumerable<PlayerNetworkInfo> PlayerNetworkInfos => playerList.Select(x => x.GetComponent<PlayerNetworkInfo>());
	public static GameObject LocalPlayer => localPlayer;

    // Start is called before the first frame update
    void Awake()
    {
	    if (Me != null)
	    {
		    Destroy(this);
		    return;
	    }
	    Me = this;
	    
        playerConfigurationManager = PlayerConfigurationManager.Me;
		networkManager = NetworkManager.Me;

		if (networkManager)
		{
			for (int i = 0; i < playerList.Length; ++i)
				InitPlayerObject (playerList[i], playerConfigurationManager.playerConfigurations[i]);
		}
		//Debug Mode
		else
		{
			for (int i = 0; i < playerList.Length; ++i)
				playerList[i].SetActive (i == 0);
		}

		localPlayer = playerList[PlayerConfigurationManager.LocalPlayerId];
    }

	private void Update()
	{
		if (networkManager)
		{
			for (int i = 0; i < playerList.Length; ++i)
				playerList[i].SetActive (playerConfigurationManager.playerConfigurations[i].connected);
		}		
	}

	private void InitPlayerObject (GameObject player, PlayerConfiguration playerConfiguration)
	{
		PlayerConfigurationContainer playerConfigurationContainer = player.GetComponentInChildren <PlayerConfigurationContainer>();
		playerConfigurationContainer.playerConfiguration = playerConfiguration;
	}

	private void OnDestroy()
	{
		if (Me == this)
		{
			Me = null;
		}
	}
}
