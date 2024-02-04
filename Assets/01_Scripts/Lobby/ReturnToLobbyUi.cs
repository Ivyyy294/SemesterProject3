using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.SceneManagement;

public class ReturnToLobbyUi : MonoBehaviour
{
	[SerializeField] GameObject ui;
	[SerializeField] GameObject buttonLobby;

	private void Start()
	{
		ui.SetActive (false);
	}

	public void ReturnToLobby ()
	{
		NetworkSceneController.Me.LoadScene (1);
	}

	public void ReturnToMenu ()
	{
		if (NetworkManager.Me)
			NetworkManager.Me.ShutDown();

		if (NetworkSceneController.Me)
		{
			NetworkSceneController.Me.Owner = true;
			NetworkSceneController.Me.LoadScene(0);
		}
		else
			SceneManager.LoadScene (0);
	}

	public void Quit()
	{
		Application.Quit();
	}

	private void Update()
	{
		if (MatchController.Me.MatchGameOver.GameOver() && !ui.activeInHierarchy)
		{
			ui.SetActive (true);
			buttonLobby.SetActive (NetworkManager.Me != null && NetworkManager.Me.Host);
		}
	}
}
