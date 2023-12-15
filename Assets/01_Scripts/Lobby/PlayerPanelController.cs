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

	bool clientConnected = false;

	public void OnClientConnected(int clientNumber, Socket socket)
	{
		clientConnected = true;
	}

    // Start is called before the first frame update
    void Start()
    {
		networkManager = NetworkManager.Me;
		playerConfiguration = PlayerConfigurationManager.Me;

		if (networkManager && networkManager.Host)
			networkManager.onClientConnected = OnClientConnected;
		
		if (playerConfiguration)
		{
			playerConfiguration.playerConfigurations[0].Owner = networkManager.Host;
			playerConfiguration.playerConfigurations[1].Owner = !networkManager.Host;
			playerPanels[0].playerConfiguration = playerConfiguration.playerConfigurations[0];
			playerPanels[1].playerConfiguration = playerConfiguration.playerConfigurations[1];
		}

        if (networkManager.Host)
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
		if (clientConnected && !playerPanels[1].gameObject.activeInHierarchy)
			playerPanels[1].gameObject.SetActive (true);

        if (playerConfiguration.playerConfigurations[0].ready && playerConfiguration.playerConfigurations[1].ready)
			SceneManager.LoadScene (2);
    }
}
