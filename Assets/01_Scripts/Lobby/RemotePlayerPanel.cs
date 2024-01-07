using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemotePlayerPanel : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration;
	[SerializeField] TextMeshProUGUI playerName;
	[SerializeField] TextMeshProUGUI labelWaiting;
	[SerializeField] TextMeshProUGUI labelReady;
	[SerializeField] GameObject colorTeam1;
	[SerializeField] GameObject colorTeam2;

    // Update is called once per frame
    void Update()
    {
		playerName.text = playerConfiguration.playerName;
        labelWaiting.gameObject.SetActive (!playerConfiguration.ready);
		labelReady.gameObject.SetActive (playerConfiguration.ready);

		colorTeam1.SetActive (playerConfiguration.teamNr == 0);
		colorTeam2.SetActive (playerConfiguration.teamNr == 1);
    }
}
