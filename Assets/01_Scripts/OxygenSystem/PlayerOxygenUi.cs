using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerOxygen), typeof (DiverInput))]
public class PlayerOxygenUi : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	bool owner;
    // Start is called before the first frame update
    void Start()
    {
        playerOxygen = GetComponent<PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
		owner = GetComponent<DiverInput>().Owner;

		if (owner)
			Debug.Log ("Oxygen: " + playerOxygen.CurrentOxygen);
    }
}
