using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenRefill : NetworkBehaviour
{
	[Range (1, 1000)]
	[SerializeField] float refillRatePerSecond = 20f;
	[SerializeField] float capacityOxygen = 0f;
	[SerializeField] float currentOxygen = 0f;

	public float CapacityOxygen => capacityOxygen;
	public float CurrentOxygen => currentOxygen;

	public void SetCurrentOxygen (float val)
	{
		currentOxygen = val;
	}

	[RPCAttribute]
	public void Despawn()
	{
		gameObject.SetActive(false);
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (currentOxygen));
	}

	private void Start()
	{
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
	}

	private void Update()
	{
		if (!Owner && networkPackage.Available)
		{
			currentOxygen = networkPackage.Value (0).GetFloat();
			networkPackage.Clear();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger || !Owner)
			return;

		PlayerOxygen playerOxygen = other.GetComponentInParent<PlayerOxygen>();

		if (playerOxygen && playerOxygen.Owner)
		{
			float refill = Mathf.Min (refillRatePerSecond * Time.deltaTime, currentOxygen);
			playerOxygen.Refill (refill);
			currentOxygen -= refill;
		}

		if (currentOxygen <= 0f)
		{
			InvokeRPC ("Despawn");
			gameObject.SetActive (false);
		}
	}
}
