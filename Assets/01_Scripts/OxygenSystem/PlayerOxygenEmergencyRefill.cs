using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygenEmergencyRefill : MonoBehaviour
{
	[SerializeField] float refillAmount = 50f;
	[SerializeField] float refillDelay = 5f;

	PlayerOxygen playerOxygen;
	float timer = 0f;

	//Gets called from OxygenEmptyBlackOutUi
	public void Refill()
	{
		playerOxygen.Refill (refillAmount);
	}

    // Start is called before the first frame update
    void Start()
    {
		playerOxygen = GetComponent<PlayerOxygen>();
    }

	private void Update()
	{
		if (playerOxygen.CurrentOxygen <= 0f)
		{
			if (timer < refillDelay)
				timer += Time.deltaTime;
			else
			{
				playerOxygen.Refill (refillAmount);
				timer = 0f;
			}
		}
	}
}
