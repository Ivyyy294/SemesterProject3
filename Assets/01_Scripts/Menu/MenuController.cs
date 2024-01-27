using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MenuController : MonoBehaviour
{
    [SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject selectPlayMode;
	[SerializeField] GameObject settings;

	public void OnPlayPressed()
	{
		selectPlayMode.SetActive (true);
	}

	public void OnSettingsPressed()
	{
		settings.SetActive (true);
	}

	public void OnCreditsPressed()
	{
		NetworkSceneController.Me.LoadScene (3);
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}

	void Start()
	{
		networkManagerCallback.ResetNetworkObjects();
		NetworkSceneController.Me.Owner = true;
	}   
}
