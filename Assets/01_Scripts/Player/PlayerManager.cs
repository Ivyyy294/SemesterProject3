using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] GameObject[] playerList = new GameObject[2];
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
	static GameObject localPlayer;

	public GameObject[] PlayerList => playerList;
	public static GameObject LocalPlayer => localPlayer;

    // Start is called before the first frame update
    void Awake()
    {
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
			SetOwnerState (playerList[0], true);

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
		SetOwnerState (player, playerConfiguration.Owner);
		PlayerConfigurationContainer playerConfigurationContainer = player.GetComponentInChildren <PlayerConfigurationContainer>();
		playerConfigurationContainer.playerConfiguration = playerConfiguration;
	}

	private void SetOwnerState (GameObject obj, bool val)
	{
		if (obj)
		{
			PlayerInput diverInput = obj.GetComponentInChildren <PlayerInput>();
			diverInput.Owner = val;

			PlayerThrowBall playerThrowBall = obj.GetComponentInChildren <PlayerThrowBall>();
			playerThrowBall.Owner = val;

			//Disable camera on remote player
			if (!val)
			{
				Camera camera = obj.GetComponentInChildren<Camera>();
				camera.gameObject.SetActive (false);
			}
			else
				localPlayer = obj;
		}
	}
}
