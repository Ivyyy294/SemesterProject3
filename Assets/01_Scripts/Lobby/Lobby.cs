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
	[SerializeField] AudioAsset startGameAudio;

	PlayerConfigurationManager configurationManager;
	NetworkSceneController networkSceneController;
	float timer = 0f;

	bool startGamePlayed = false;

	public float TimeUntilStart => timer;

	public bool AllPayersReady()
	{
		 return configurationManager && configurationManager.PlayersReady() && configurationManager.EqualTeamSize();
	}

	private void Start()
	{
		startGamePlayed = false;
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
		{
			timer -= Time.deltaTime;

			if (!startGamePlayed)
			{
				startGamePlayed = true;
				startGameAudio?.PlayOneShot();
			}
		}
		else
			timer = matchStartDelay;

		if (AllPayersReady() && timer <= 0f)
			networkSceneController.LoadScene(2);
	}
}
