using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ImageTeamColor : MonoBehaviour
{
    [SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] Image team1;
	[SerializeField] Image team2;

    // Start is called before the first frame update
    void Start()
    {
		team1.color = teamColorSettings.GetTeamColor(0);
		team2.color = teamColorSettings.GetTeamColor(1);
    }
}
