using Ivyyy.Network;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public struct HostSessionData
{
	public string lobbyName;
	public string ip;
}

public class NetworkManagerHostSessionExplorer
{
    Task findhostSessionTask;
	bool exitSearch = false;
	UdpClient udpClient;
	int port;
	NetworkPackage networkPackage = new NetworkPackage();

	List <HostSessionData> hostSessionList = new List <HostSessionData>();

	public List <HostSessionData> HostSessionList => hostSessionList;

	public NetworkManagerHostSessionExplorer()
	{
		udpClient = new UdpClient();
	}

	public void ShutDownSearchHostSession()
	{
		exitSearch = true;
		findhostSessionTask.Wait();
		findhostSessionTask = null;
		udpClient.Close();
		udpClient.Dispose();
		HostSessionList.Clear();
	}

	public void StartSearchHostSession()
	{
		if (findhostSessionTask != null)
			return;

		exitSearch = false;
		port = NetworkManager.Me.Port + 1;
		udpClient = new UdpClient();
		udpClient.EnableBroadcast = true;
		udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
		findhostSessionTask = Task.Run(()=> {SearchHostSession();});
	}

	void SearchHostSession()
	{
		var iPEndPoint = new IPEndPoint(0, 0);

		while (!exitSearch)
		{
			if (udpClient.Available > 0)
			{
				var recvBuffer = udpClient.Receive(ref iPEndPoint);
				networkPackage.DeserializeData (recvBuffer);

				if (networkPackage.Available)
				{
					string lobbyName = networkPackage.Value(0).GetString();
					string hostName = networkPackage.Value(1).GetString();
					string ipString = iPEndPoint.Address.ToString();
					string ipString2 = ResolveHostNameToIp (hostName);
					AddHostSession(lobbyName, ipString);
				}
			}
		}
	}

	void AddHostSession (string lobbyName, string ipString)
	{
		try
		{
			HostSessionData hostSession = new HostSessionData();
			hostSession.lobbyName = lobbyName;
			hostSession.ip = ipString;

			if (!hostSessionList.Contains (hostSession))
				hostSessionList.Add (hostSession);
		}
		catch
		{
			Debug.LogError("Couldn't resolve Host IP!");
		}
	}

	string ResolveHostNameToIp (string hostName)
	{
		var host = Dns.GetHostEntry(hostName);
		string ipString = "";

		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				ipString = ip.ToString();
				break;
			}
		}

		return ipString;
	}
}
