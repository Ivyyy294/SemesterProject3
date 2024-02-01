using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MenuController : MonoBehaviour
{
    [SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject selectPlayMode;
	[SerializeField] GameObject startScreen;

	public void OnShowStartScreen()
	{
		selectPlayMode.SetActive (false);
		startScreen.SetActive (true);
	}

	public void OnPlayPressed()
	{
		selectPlayMode.SetActive (true);
		startScreen.SetActive (false);
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
	}   
}
