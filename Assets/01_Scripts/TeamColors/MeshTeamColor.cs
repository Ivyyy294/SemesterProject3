using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshTeamColor : MonoBehaviour
{
	[SerializeField] TeamColorSettings teamColorSettings;
	[SerializeField] int teamIndex;
	Renderer mRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mRenderer = GetComponent<Renderer>();
		mRenderer.sharedMaterial.EnableKeyword("_EMISSION");

		Color color = teamColorSettings.GetTeamColor (teamIndex);

		mRenderer.sharedMaterial.color = color;
		mRenderer.sharedMaterial.SetColor("_EmissionColor", color);
    }
}
