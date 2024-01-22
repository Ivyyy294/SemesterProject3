using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject selectHostSessionObj;
	[SerializeField] GameObject enterHostAdddressObj;
	[SerializeField] GameObject selectPlaymodeObj;
	[SerializeField] GameObject enterLobbyNameObj;

	public void Start()
	{
		networkManagerCallback.ResetNetworkObjects();
	}

	public void OnHostPressed()
	{
		enterLobbyNameObj.SetActive (true);
		selectPlaymodeObj.SetActive (false);
	}

	public void OnJoinPressed()
	{
		selectHostSessionObj.SetActive (true);
		selectPlaymodeObj.SetActive (false);
	}

	public void OnEnterIpPressed()
	{
		enterHostAdddressObj.SetActive (true);
		selectPlaymodeObj.SetActive (false);
	}

	public void ShowPlayModeSelection()
	{
		enterLobbyNameObj.SetActive (false);
		enterHostAdddressObj.SetActive (false);
		selectHostSessionObj.SetActive (false);
		selectPlaymodeObj.SetActive (true);
	}
}
