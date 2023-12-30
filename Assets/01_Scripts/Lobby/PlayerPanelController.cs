using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ivyyy.Network;
using System.Net.Sockets;

public class PlayerPanelController : MonoBehaviour
{
	[SerializeField] PlayerPanel[] playerPanels = new PlayerPanel[2];
	NetworkManager networkManager;
	PlayerConfigurationManager playerConfiguration;

    // Start is called before the first frame update
    void Start()
    {
		networkManager = NetworkManager.Me;
		bool isHost = networkManager.Host;
		playerConfiguration = PlayerConfigurationManager.Me;
		
		//Init Panels and PlayerConfiguration Owner state
		for (int i = 0; i < playerConfiguration.MaxPlayerCount; ++i)
		{
			PlayerPanel playerPanel = playerPanels[i];
			playerPanel.playerConfiguration = playerConfiguration.playerConfigurations[i];
		}
	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < playerConfiguration.MaxPlayerCount; ++i)
		{
			if (playerConfiguration.IsClientConnected(i) && !playerPanels[i].gameObject.activeInHierarchy)
				playerPanels[i].gameObject.SetActive (true);
		}
	}
}
