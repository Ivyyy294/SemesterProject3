using Ivyyy.Network;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManagerCallback : MonoBehaviour
{
	[SerializeField] NetworkManagerUi networkManagerUi;
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
    
	public bool OnAcceptClient (Socket socket)
	{
		IPAddress iPAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;

		return playerConfigurationManager.GetNewPlayerIndex (iPAddress) != -1;
	}

	// Start is called before the first frame update
    void Start()
    {
        playerConfigurationManager = PlayerConfigurationManager.Me;
		
		if (networkManager)
		{
			//networkManager.acceptClient = OnAcceptClient;
		}
    }
}
