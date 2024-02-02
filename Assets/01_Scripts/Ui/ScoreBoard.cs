using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
	[SerializeField] List <ScoreBoardEntry> scoreBoardEntries;

	private void OnEnable()
	{
		if (PlayerConfigurationManager.Me)
		{
			for (int i = 0; i < scoreBoardEntries.Count && i < PlayerConfigurationManager.Me.MaxPlayerCount; ++i)
				scoreBoardEntries[i].Show (i);
		}
		else if (scoreBoardEntries.Count > 0)
			scoreBoardEntries[0].Show (0);
	}
}