using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygenEmergencyRefill : MonoBehaviour
{
	[SerializeField] float refillAmount = 50f;
	[SerializeField] float refillDelay = 5f;

	PlayerOxygen playerOxygen;

	float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
		playerOxygen = GetComponent<PlayerOxygen>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (playerOxygen.Owner && playerOxygen.OxygenEmpty)
		{
			if (timer < refillDelay)
				timer += Time.deltaTime;
			else
				playerOxygen.Refill (refillAmount);
		}
		else if (timer != 0f)
			timer = 0f;
    }
}
