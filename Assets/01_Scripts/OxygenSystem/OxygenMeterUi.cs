using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OxygenMeterUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI oxygenLabel;
	[SerializeField] PlayerManager playerManager;
	PlayerOxygen playerOxygen;
	int localPlayerId = 0;

    // Start is called before the first frame update
    void Start()
    {
		if (PlayerConfigurationManager.Me)
			localPlayerId = PlayerConfigurationManager.Me.LocalPlayerId;

		if (playerManager)
			playerOxygen = playerManager.PlayerList[localPlayerId].GetComponentInChildren<PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenLabel && playerOxygen)
			oxygenLabel.text = ((int)playerOxygen.CurrentOxygenPercent).ToString();
    }
}
