using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterLobbyName : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] TMP_InputField inputLobbyName;
    
	public void OnStartPressed()
	{
		networkManagerCallback.OnHostStarted (inputLobbyName.text);
	}
}
