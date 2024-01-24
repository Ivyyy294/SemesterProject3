using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class ReturnToLobbyUi : MonoBehaviour
{
	[SerializeField] GameObject ui;

	private void Start()
	{
		if (!NetworkManager.Me || !NetworkManager.Me.Host)
			enabled = false;

		ui.SetActive (false);
	}

	public void ReturnToLobby ()
	{
		NetworkSceneController.Me.LoadScene (1);
	}

	private void Update()
	{
		if (MatchController.Me.MatchGameOver.GameOver() && !ui.activeInHierarchy)
			ui.SetActive (true);
	}
}
