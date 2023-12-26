using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Ivyyy.Network;

public class Lobby : MonoBehaviour
{
	PlayerConfigurationManager configurationManager;
	NetworkSceneController networkSceneController;

	private void Start()
	{
		configurationManager = PlayerConfigurationManager.Me;
		networkSceneController = NetworkSceneController.Me;

		if (configurationManager)
			configurationManager.ResetPlayers();
	}

	private void Update()
	{
		if (configurationManager && configurationManager.PlayersReady())
			networkSceneController.LoadScene(2);
	}
}
