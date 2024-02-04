using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;

public class EnterLobbyName : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] TMP_InputField inputLobbyName;
    
	public void OnStartPressed (int playerCount)
	{
		PlayerConfigurationManager.Me.MaxPlayerCount = playerCount;
		networkManagerCallback.OnHostStarted (inputLobbyName.text);
	}
}
