using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OxygenMeterUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI oxygenLabel;
	[SerializeField] PlayerManager playerManager;
	PlayerOxygen playerOxygen;

    // Start is called before the first frame update
    void Start()
    {
		if (playerManager)
			playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenLabel && playerOxygen)
			oxygenLabel.text = ((int)playerOxygen.CurrentOxygenPercent).ToString();
    }
}
