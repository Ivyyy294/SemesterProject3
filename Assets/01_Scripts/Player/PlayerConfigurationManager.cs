using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Net.Sockets;
using System;

public class PlayerConfigurationManager : MonoBehaviour
{
	[SerializeField] PlayerConfiguration[] playerConfigurations = new PlayerConfiguration[2];
	public int LocalPlayerId { get; set;}
	static public PlayerConfigurationManager Me {get; private set;}

	private void Awake()
	{
		if (Me == null)
		{
			Me = this;
			DontDestroyOnLoad (this);
		}
		else
			Destroy (this);
	}
}
