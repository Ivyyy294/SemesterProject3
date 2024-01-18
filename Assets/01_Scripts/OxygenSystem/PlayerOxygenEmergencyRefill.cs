using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygenEmergencyRefill : MonoBehaviour
{
	[SerializeField] float refillAmount = 50f;

	PlayerOxygen playerOxygen;

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
}
