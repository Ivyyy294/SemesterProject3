using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTeamColor : MonoBehaviour
{
	[SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] Button team1;
	[SerializeField] Button team2;

    // Start is called before the first frame update
	[ExecuteInEditMode]
    void Start()
    {
		ColorBlock colorBlock = team1.colors;
        colorBlock.normalColor = teamColorSettings.GetTeamColor(0);
		team1.colors = colorBlock;

		colorBlock.normalColor = teamColorSettings.GetTeamColor(1);
		team2.colors = colorBlock;
    }
}
