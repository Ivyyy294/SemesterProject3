using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenRefill : MonoBehaviour
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

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger)
			return;

		PlayerOxygen playerOxygen = other.GetComponentInParent<PlayerOxygen>();

		if (playerOxygen && playerOxygen.Owner)
		{
			float refill = Mathf.Min (refillRatePerSecond * Time.deltaTime, currentOxygen);
			playerOxygen.Refill (refill);
			currentOxygen -= refill;
		}

		if (currentOxygen <= 0f)
			gameObject.SetActive(false);
	}
}
