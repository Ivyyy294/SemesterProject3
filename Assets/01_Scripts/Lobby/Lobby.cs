using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Ivyyy.Network;
using Ivyyy.GameEvent;

public class Lobby : MonoBehaviour
{
	[SerializeField] float matchStartDelay;
	[SerializeField] GameEvent audioAmbient;

	PlayerConfigurationManager configurationManager;
	NetworkSceneController networkSceneController;
	float timer = 0f;

	public float TimeUntilStart => timer;

	public bool AllPayersReady()
	{
		 return configurationManager && configurationManager.PlayersReady() && configurationManager.EqualTeamSize();
	}

	private void Start()
	{
		configurationManager = PlayerConfigurationManager.Me;
		networkSceneController = NetworkSceneController.Me;

		if (configurationManager)
			configurationManager.ResetPlayers();

		audioAmbient?.Raise();
	}

	private void Update()
	{
		bool playersReady = AllPayersReady();

		if (playersReady)
			timer -= Time.deltaTime;
		else
			timer = matchStartDelay;

		if (AllPayersReady() && timer <= 0f)
			networkSceneController.LoadScene(2);
	}
}
