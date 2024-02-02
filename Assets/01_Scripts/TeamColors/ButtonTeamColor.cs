using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTeamColor : MonoBehaviour
{
	[SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] Button team1;
	[SerializeField] Button team2;

	int localPlayerId;
    // Start is called before the first frame update
	[ExecuteInEditMode]
    void Start()
    {
		SetButtonColor (team1, 0);
		SetButtonColor (team2, 1);

		localPlayerId = PlayerConfigurationManager.LocalPlayerId;
    }

	private void Update()
	{
		if (PlayerConfigurationManager.LocalPlayerTeamIndex == 0)
		{
			SetActiveTeamButton (team1, 0);
			SetInActiveTeamButton (team2, 1);
		}
		else if (PlayerConfigurationManager.LocalPlayerTeamIndex == 1)
		{
			SetActiveTeamButton (team2, 1);
			SetInActiveTeamButton (team1, 0);
		}
	}

	void SetButtonColor (Button button, int index)
	{
		ColorBlock colorBlock = button.colors;
		Color teamColor = teamColorSettings.GetTeamColor(index);
		Color teamColorGrey = DesaturateColor (teamColor);
		
		colorBlock.selectedColor = teamColor;
		colorBlock.highlightedColor = teamColor;
		colorBlock.pressedColor = teamColor;
		colorBlock.normalColor = teamColorGrey;
		button.colors = colorBlock;
	}

	void SetActiveTeamButton(Button button, int teamIndex)
	{
		ColorBlock colorBlock = button.colors;
		Color teamColor = teamColorSettings.GetTeamColor(teamIndex);
		colorBlock.normalColor = teamColor;
		button.colors = colorBlock;
	}

	void SetInActiveTeamButton(Button button, int teamIndex)
	{
		ColorBlock colorBlock = button.colors;
		Color teamColor = teamColorSettings.GetTeamColor(teamIndex);
		colorBlock.normalColor = DesaturateColor (teamColor);
		button.colors = colorBlock;
	}

	Color DesaturateColor(Color originalColor)
    {
        float h, s, v;
        Color.RGBToHSV(originalColor, out h, out s, out v);
        
        // Reduce the saturation
        s *= 0.25f;
        
        // Ensure saturation is within the valid range [0, 1]
        s = Mathf.Clamp01(s);

        // Convert back to RGB
        return Color.HSVToRGB(h, s, v);
    }
}
