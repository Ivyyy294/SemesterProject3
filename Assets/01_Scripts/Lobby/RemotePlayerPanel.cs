using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemotePlayerPanel : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration;

	[SerializeField] TextMeshProUGUI playerName;

	[SerializeField] GameObject arrowReady;
	[SerializeField] GameObject decorationReady;

	[SerializeField] TeamColorSettings colorSettings;
	[SerializeField] Image colorTeam;

    // Update is called once per frame
    void Update()
    {
		playerName.text = playerConfiguration.playerName;

		arrowReady.gameObject.SetActive (playerConfiguration.ready);
        decorationReady.gameObject.SetActive (playerConfiguration.ready);

		colorTeam.color = colorSettings.GetTeamColor (playerConfiguration.teamNr);
    }
}
