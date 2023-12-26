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

	public bool AllPlayersReady()
	{
		return playerConfiguration.playerConfigurations[0].ready && playerConfiguration.playerConfigurations[1].ready;
	}

    // Start is called before the first frame update
    void Start()
    {
		networkManager = NetworkManager.Me;
		bool isHost = networkManager.Host;
		playerConfiguration = PlayerConfigurationManager.Me;
		
		if (playerConfiguration)
		{
			playerConfiguration.playerConfigurations[0].Owner = isHost;
			playerConfiguration.playerConfigurations[1].Owner = !isHost;
			playerPanels[0].playerConfiguration = playerConfiguration.playerConfigurations[0];
			playerPanels[1].playerConfiguration = playerConfiguration.playerConfigurations[1];
		}

        if (isHost)
			playerPanels[0].gameObject.SetActive (true);
		else
		{
			playerPanels[0].gameObject.SetActive (true);
			playerPanels[1].gameObject.SetActive (true);
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (playerConfiguration.ClientConnected && !playerPanels[1].gameObject.activeInHierarchy)
			playerPanels[1].gameObject.SetActive (true);
    }
}
