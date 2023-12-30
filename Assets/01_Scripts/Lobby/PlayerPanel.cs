using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.UI;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration;

	[SerializeField] GameObject readyButton;
	[SerializeField] TextMeshProUGUI labelWaiting;
	[SerializeField] TextMeshProUGUI labelReady;

	public void OnReadyButtonPressed()
	{
		playerConfiguration.ready = true;
	}

	private void OnEnable()
	{
		if (playerConfiguration.Owner)
		{
			labelWaiting.gameObject.SetActive (false);
			labelReady.gameObject.SetActive (false);
		}
		else
		{
			readyButton.SetActive (false);
			labelReady.gameObject.SetActive (false);
		}
	}

	private void Update()
	{
		readyButton.SetActive (playerConfiguration.Owner && !playerConfiguration.ready);
		labelWaiting.gameObject.SetActive (!playerConfiguration.Owner && !playerConfiguration.ready);
		labelReady.gameObject.SetActive (playerConfiguration.ready);
	}
}
