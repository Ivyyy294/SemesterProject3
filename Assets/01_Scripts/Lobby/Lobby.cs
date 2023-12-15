using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
	PlayerConfigurationManager configurationManager;

	private void Start()
	{
		configurationManager = PlayerConfigurationManager.Me;

		if (configurationManager)
			configurationManager.ResetPlayers();
	}

	private void Update()
	{
		if (configurationManager && configurationManager.PlayersReady())
			SceneManager.LoadScene(2);
	}
}
