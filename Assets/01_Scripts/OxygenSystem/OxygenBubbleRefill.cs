using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBubbleRefill : MonoBehaviour
{
	[Range (1, 1000)]
	[SerializeField] float capacityOxygen = 0f;
	[Range (1, 1000)]
	[SerializeField] float refillRatePerSecond = 20f;

	[Header ("Oxygen loss settings")]
	[SerializeField] bool depletableByPlayers = true;
	[Min (0)]
	[SerializeField] float oxygenLossDelay;
	[Min (0)]
	[SerializeField] float oxygenLossPerSecond;
	
	//Private values
	float oxygenLossTimer = 0f;
	float currentOxygen = -1f;

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

	//Private Methods
	private void Start()
	{
		if (currentOxygen == -1)
			currentOxygen = capacityOxygen;
	}

	private void Update()
	{
		OxygenLoss();

		//Despawn if oxygen is empty
		if (currentOxygen <= 0f)
			gameObject.SetActive (false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
			return;

		PlayerCollision playerCollision = other.transform.parent.GetComponent<PlayerCollision>();

		if (playerCollision)
		{
			PlayerOxygen playerOxygen = playerCollision.PlayerOxygen;
			playerOxygen.PlayerAudioInhale();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger || !other.CompareTag ("Player"))
			return;

		RefillPlayerOxygen (other);
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

	private void RefillPlayerOxygen (Collider other)
	{
		PlayerCollision playerCollision = other.transform.parent.GetComponent<PlayerCollision>();

		if (playerCollision)
		{
			PlayerOxygen playerOxygen = playerCollision.PlayerOxygen;

			if (playerOxygen.Owner)
			{
				float refill = Mathf.Min (refillRatePerSecond * Time.deltaTime, currentOxygen, playerOxygen.MissingOxygen);
				playerOxygen.Refill (refill);

				if (depletableByPlayers)
					currentOxygen -= refill;
			}
		}
	}
}
