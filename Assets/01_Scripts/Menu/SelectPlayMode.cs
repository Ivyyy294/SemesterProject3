using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Network;
using UnityEngine.SceneManagement;
using System;

public class SelectPlayMode : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI  ip;
	[SerializeField] TextMeshProUGUI port;

	public void OnHostPressed()
	{
		NetworkManager.Me.StartHost (23000);
		SceneManager.LoadScene (1);
	}

	public void OnJoinPressed()
	{
		if (NetworkManager.Me.StartClient ("127.0.0.1" ,23000));
			SceneManager.LoadScene (2);
	}
}
