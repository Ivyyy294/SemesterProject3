using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewTeamColorSettings", menuName = "TeamColorAsset")]
public class TeamColorSettings : ScriptableObject
{
	[SerializeField] private List<Color> teamColors;
	[SerializeField] float cycleDuration = 0.5f;

	public Color GetTeamColor (int teamIndex)
	{
		return teamColors[teamIndex];
	}

	public Color CycleRainbowColors()
    {
        float t = Mathf.PingPong(Time.time / cycleDuration, 1f); // PingPong between 0 and 1
        Color rainbowColor = HSVToRGB(t, 1f, 1f);
        return rainbowColor;
    }

    private Color HSVToRGB(float h, float s, float v)
    {
        float hclamped = Mathf.Repeat(h, 1f);
        return Color.HSVToRGB(hclamped, s, v);
    }
}
