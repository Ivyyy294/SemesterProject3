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
		 return configurationManager && configurationManager.PlayersReady();
	}

	public bool EqualTeamSize()
	{
		return configurationManager.EqualTeamSize();
	}

	private void Start()
	{
		Cursor.visible = true;
		startGamePlayed = false;
		configurationManager = PlayerConfigurationManager.Me;
		networkSceneController = NetworkSceneController.Me;

		if (configurationManager)
			configurationManager.ResetPlayers();

		audioAmbient?.Raise();
	}

	private void Update()
	{
		bool playersReady = AllPayersReady() && EqualTeamSize();

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

		if (AllPayersReady() && EqualTeamSize() && timer <= 0f)
			networkSceneController.LoadScene(2);
	}
}
