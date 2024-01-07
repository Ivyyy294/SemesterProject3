using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] GameObject[] playerList = new GameObject[2];
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {
        playerConfigurationManager = PlayerConfigurationManager.Me;
		networkManager = NetworkManager.Me;

		if (networkManager)
		{
			for (int i = 0; i < playerList.Length; ++i)
				InitPlayerObject (playerList[i], playerConfigurationManager.playerConfigurations[i]);
		}
		else
			InitPlayerObject (playerList[0], playerConfigurationManager.playerConfigurations[0]);
    }

	private void Update()
	{
		for (int i = 0; i < playerList.Length; ++i)
			playerList[i].SetActive (playerConfigurationManager.playerConfigurations[i].connected);
	}

	private void InitPlayerObject (GameObject player, PlayerConfiguration playerConfiguration)
	{
		SetOwnerState (player, playerConfiguration.Owner);

		PlayerTeam playerTeam = player.GetComponent<PlayerTeam>();
		playerTeam.playerConfiguration = playerConfiguration;
	}

	private void SetOwnerState (GameObject obj, bool val)
	{
		if (obj)
		{
			DiverInput diverInput = obj.GetComponent <DiverInput>();
			diverInput.Owner = val;

			PlayerThrowBall playerThrowBall = obj.GetComponent <PlayerThrowBall>();
			playerThrowBall.Owner = val;

			//Disable camera on remote player
			if (!val)
			{
				Camera camera = obj.GetComponentInChildren<Camera>();
				camera.gameObject.SetActive (false);
			}
		}
	}
}
