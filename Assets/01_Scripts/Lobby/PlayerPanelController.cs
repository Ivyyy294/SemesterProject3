using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ivyyy.Network;
using System.Net.Sockets;

public class PlayerPanelController : MonoBehaviour
{
	[SerializeField] RemotePlayerPanel[] remotePlayerPanels = new RemotePlayerPanel[3];
	[SerializeField] LocalPlayerPanel localPlayerPanel;

	NetworkManager networkManager;
	PlayerConfigurationManager playerConfiguration;

    // Start is called before the first frame update
    void Start()
    {
		networkManager = NetworkManager.Me;
		bool isHost = !networkManager || networkManager.Host;
		playerConfiguration = PlayerConfigurationManager.Me;

		InitPlayerPanels();
	}

	// Update is called once per frame
	void Update()
	{
		foreach (RemotePlayerPanel remotePlayerPanel in remotePlayerPanels)
		{
			bool connected = remotePlayerPanel.playerConfiguration != null && remotePlayerPanel.playerConfiguration.connected;

			if (connected != remotePlayerPanel.gameObject.activeInHierarchy)
				remotePlayerPanel.gameObject.SetActive (connected);
		}
	}

	void InitPlayerPanels()
	{
		Stack <PlayerConfiguration> remotePlayerConfigurations = new Stack<PlayerConfiguration>();

		//Init Panels and PlayerConfiguration Owner state
		for (int i = 0; i < playerConfiguration.MaxPlayerCount; ++i)
		{
			PlayerConfiguration pc = playerConfiguration.playerConfigurations[i];

			if (pc.Owner)
			{
				localPlayerPanel.playerConfiguration = pc;
				localPlayerPanel.gameObject.SetActive (true);
			}
			else
				remotePlayerConfigurations.Push (pc);
		}

		int j = 0;
		while (remotePlayerConfigurations.Count > 0)
		{
			PlayerConfiguration pc = remotePlayerConfigurations.Pop();
			remotePlayerPanels[j++].playerConfiguration = pc;
		}

	}
}
