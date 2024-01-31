using Ivyyy.Network;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManagerHostSessionBroadcast
{
	Task hostSessionBroadcastTask;
	bool exitHost = false;
	UdpClient udpClient;
	int port;
	NetworkPackage networkPackage = new NetworkPackage();

	public NetworkManagerHostSessionBroadcast()
	{
		port = NetworkManager.Me.Port + 1;
		udpClient = new UdpClient();
		udpClient.EnableBroadcast = true;
		//udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
	}

	public void ShutDownHostSessionBroadcast()
	{
		exitHost = true;
		hostSessionBroadcastTask.Wait();
		hostSessionBroadcastTask = null;
	}

	public void StartHostSessionBroadcast (string lobbyName)
	{
		if (hostSessionBroadcastTask != null)
			ShutDownHostSessionBroadcast();

		exitHost = false;
		hostSessionBroadcastTask = Task.Run(()=> {HostSessionBroadcast (lobbyName);});
	}

	//Private Methods
	void HostSessionBroadcast (string lobbyName)
	{
		networkPackage.Clear();
		networkPackage.AddValue (new NetworkPackageValue(lobbyName));
		networkPackage.AddValue (new NetworkPackageValue(Dns.GetHostName()));

		byte[] data = networkPackage.GetSerializedData();

		while (!exitHost)
		{
			if (!NetworkManager.Me.Host)
			{
				exitHost = true;
				Debug.Log ("Exit Host Session broadcast!");
			}
			else
			{
				udpClient.Send(data, data.Length, "255.255.255.255", port);
				Thread.Sleep (1000);
			}
		}
	}
}
