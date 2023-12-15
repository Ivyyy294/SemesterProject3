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

	bool clientConnected = false;

	public void OnClientConnected(int clientNumber, Socket socket)
	{
		clientConnected = true;
	}

    // Start is called before the first frame update
    void Start()
    {
		networkManager = NetworkManager.Me;

		if (networkManager && networkManager.Host)
			networkManager.onClientConnected = OnClientConnected;
		
		playerPanels[0].Owner = networkManager.Host;
		playerPanels[1].Owner = !networkManager.Host;

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

        if (playerPanels[0].Ready && playerPanels[1].Ready)
			SceneManager.LoadScene (2);
    }
}
