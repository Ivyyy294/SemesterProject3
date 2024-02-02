using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardEntry : MonoBehaviour
{
	[SerializeField] TeamColorSettings colorSettings;
	[SerializeField] TextMeshProUGUI nameTxt;
	[SerializeField] TextMeshProUGUI pointTxt;

	public void Show (int playerIndex)
	{
		PlayerConfiguration playerConfiguration = PlayerConfigurationManager.Me ? PlayerConfigurationManager.Me.playerConfigurations[playerIndex] : null;

		Color color = colorSettings.GetTeamColor (playerConfiguration != null ? playerConfiguration.teamNr : 0);
		nameTxt.color = color;
		pointTxt.color = color;

		//Init Name
		if (playerConfiguration)
			nameTxt.text = playerConfiguration.playerName;
		else
			nameTxt.text = PlayerConfigurationManager.LocalPlayerName;

		//Init Score
		pointTxt.text = MatchController.Me.MatchScoreController.PlayerScoreCount [playerIndex].ToString();

		gameObject.SetActive (true);
	}
}
