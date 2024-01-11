using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenLowUi : MonoBehaviour
{
    [SerializeField] GameObject uiObj;
	[SerializeField] PlayerManager playerManager;
	PlayerOxygen playerOxygen;

    // Start is called before the first frame update
    void Start()
    {
		if (playerManager)
			playerOxygen = playerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiObj && playerOxygen)
			uiObj.SetActive (playerOxygen.OxygenEmpty);
    }
}
