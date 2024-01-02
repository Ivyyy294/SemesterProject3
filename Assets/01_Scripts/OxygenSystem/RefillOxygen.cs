using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
public class RefillOxygen : MonoBehaviour
{
	[Range (1, 1000)]
	[SerializeField] float instantRefill = 20f;

	[Range (1, 100)]
	[SerializeField] float stayRefillRate = 5f;

	private void OnTriggerEnter(Collider other)
	{
		PlayerOxygen playerOxygen = other.GetComponentInParent<PlayerOxygen>();

		if (playerOxygen && playerOxygen.Owner)
			playerOxygen.Refill (instantRefill);
	}

	private void OnTriggerStay(Collider other)
	{
		PlayerOxygen playerOxygen = other.GetComponentInParent<PlayerOxygen>();

		if (playerOxygen && playerOxygen.Owner)
			playerOxygen.Refill (stayRefillRate * Time.deltaTime);
	}
}
