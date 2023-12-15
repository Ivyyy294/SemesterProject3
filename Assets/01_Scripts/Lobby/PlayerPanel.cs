using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.UI;
using TMPro;

public class PlayerPanel : NetworkBehaviour
{
	[SerializeField] GameObject readyButton;
	[SerializeField] TextMeshProUGUI labelWaiting;
	[SerializeField] TextMeshProUGUI labelReady;

	public bool Ready { get { return ready;} }
	bool ready = false;

	public void OnReadyButtonPressed()
	{
		readyButton.SetActive (false);
		labelReady.gameObject.SetActive (true);
		ready = true;
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (ready));
	}

	private void OnEnable()
	{
		ready = false;

		if (Owner)
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
		if (!Owner && networkPackage.Count > 0)
		{
			ready = networkPackage.Value (0).GetBool();

			labelWaiting.gameObject.SetActive (!ready);
			labelReady.gameObject.SetActive (ready);
		}
	}
}
