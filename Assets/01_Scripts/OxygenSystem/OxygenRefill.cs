using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenRefill : NetworkBehaviour
{
	[Range (1, 1000)]
	[SerializeField] float capacityOxygen = 0f;
	[Range (1, 1000)]
	[SerializeField] float refillRatePerSecond = 20f;

	[Header ("Oxygen loss settings")]
	[Min (0)]
	[SerializeField] float oxygenLossDelay;
	[Min (0)]
	[SerializeField] float oxygenLossPerSecond;
	
	//Private values
	float oxygenLossTimer = 0f;
	float currentOxygen = 0f;

	//Public Methods
	public float CapacityOxygen => capacityOxygen;
	public float CurrentOxygen => currentOxygen;

	public void SetCurrentOxygen (float val)
	{
		currentOxygen = val;

		if (currentOxygen > capacityOxygen)
		{
			Debug.LogError("CurrentOxygen is greater as capacityOxygen!");
			currentOxygen = capacityOxygen;
		}
	}

	[RPCAttribute]
	public void Despawn()
	{
		gameObject.SetActive(false);
	}

	//Protected Methods
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (currentOxygen));
	}

	//Private Methods
	private void Start()
	{
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
		currentOxygen = capacityOxygen;
	}

	private void Update()
	{
		if (!Owner && networkPackage.Available)
		{
			currentOxygen = networkPackage.Value (0).GetFloat();
			networkPackage.Clear();
		}
		else if (Owner)
		{
			OxygenLoss();

			//Despawn if oxygen is empty
			if (currentOxygen <= 0f)
			{
				InvokeRPC ("Despawn");
				gameObject.SetActive (false);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger || !Owner)
			return;

		PlayerCollision playerCollision = other.transform.parent.GetComponent<PlayerCollision>();

		if (playerCollision)
		{
			PlayerOxygen playerOxygen = playerCollision.PlayerOxygen;

			if (playerOxygen.Owner)
			{
				float refill = Mathf.Min (refillRatePerSecond * Time.deltaTime, currentOxygen, playerOxygen.MissingOxygen);
				playerOxygen.Refill (refill);
				currentOxygen -= refill;
			}
		}
	}

	private void OnEnable()
	{
		oxygenLossTimer = 0f;
	}

	private void OxygenLoss()
	{
		if (oxygenLossDelay <= 0f)
			return;

		if (oxygenLossTimer < oxygenLossDelay)
			oxygenLossTimer += Time.deltaTime;
		else
			currentOxygen -= oxygenLossPerSecond * Time.deltaTime;
	}
}
