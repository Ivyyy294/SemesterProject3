using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTeamColor : MonoBehaviour
{
	[SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] int teamIndex;
	Renderer mRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mRenderer = GetComponent<Renderer>();
		mRenderer.material.color = teamColorSettings.GetTeamColor (teamIndex);
    }
}
