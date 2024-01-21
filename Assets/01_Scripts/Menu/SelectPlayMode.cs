using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject selectHostSessionObj;
	[SerializeField] GameObject selectPlaymodeObj;

	public void Start()
	{
		networkManagerCallback.ResetNetworkObjects();
		selectHostSessionObj.GetComponent<SelectHostSession>().backButton.onClick.AddListener (()=>{ShowPlayModeSelection();});
	}

	public void OnHostPressed()
	{
		networkManagerCallback.OnHostStarted();
	}

	public void OnJoinPressed()
	{
		selectHostSessionObj.SetActive (true);
		selectPlaymodeObj.SetActive (false);
	}

	void ShowPlayModeSelection()
	{
		selectHostSessionObj.SetActive (false);
		selectPlaymodeObj.SetActive (true);
	}
}
