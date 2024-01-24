using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewTeamColorSettings", menuName = "TeamColorAsset")]
public class TeamColorSettings : ScriptableObject
{
	[SerializeField] private List<Color> teamColors;

	public Color GetTeamColor (int teamIndex)
	{
		return teamColors[teamIndex];
	}
}
