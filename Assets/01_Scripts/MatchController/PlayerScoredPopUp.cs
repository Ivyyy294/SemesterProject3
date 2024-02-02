using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoredPopUp : MonoBehaviour
{
	[SerializeField] float lifeTime = 1f;
	[SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] TextMeshProUGUI text;

	private void OnEnable()
	{
		int playerId = MatchController.Me.MatchScoreController.LastScoringPlayer;

		PlayerConfiguration playerConfiguration = PlayerConfigurationManager.Me ? PlayerConfigurationManager.Me.playerConfigurations[playerId] : null;

		int teamIndex = playerConfiguration ? playerConfiguration.teamNr : 0;

		Color color = teamColorSettings.GetTeamColor (teamIndex);
		text.color = color;

		if (playerId < 0)
			text.text = "Crawly scored!";
		else
		{
			string name = playerConfiguration ? playerConfiguration.playerName : PlayerConfigurationManager.LocalPlayerName;
			text.text = name + " scored!";
		}

		StartCoroutine (Disable());
	}

	IEnumerator Disable()
	{
		float timer = 0f;

		while (timer < lifeTime)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		gameObject.SetActive(false);
	}
}
