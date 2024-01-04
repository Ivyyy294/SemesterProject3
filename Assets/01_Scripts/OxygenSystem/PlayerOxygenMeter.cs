using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OxygenLevelIndicator
{
	public float oxygenLevelPercent;
	public Material material;
}

[RequireComponent (typeof (PlayerOxygen))]
public class PlayerOxygenMeter : MonoBehaviour
{
	[SerializeField] List <OxygenLevelIndicator> oxygenLevelIndicators = new List<OxygenLevelIndicator>();

	[Header ("Lara Values")]
	[SerializeField] GameObject OxygenMeterObj;

	PlayerOxygen playerOxygen;
	Renderer OxygenMeterRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerOxygen = GetComponent<PlayerOxygen>();
		OxygenMeterRenderer = OxygenMeterObj.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
		if (OxygenMeterRenderer)
		{
			Material currentMaterial = GetActiveMaterial();

			if (currentMaterial)
				OxygenMeterRenderer.material = currentMaterial;
		}
    }

	Material GetActiveMaterial()
	{
		float currentOxygenLevel = playerOxygen.CurrentOxygenPercent;

		foreach (var i in oxygenLevelIndicators)
		{
			if (currentOxygenLevel >= i.oxygenLevelPercent)
				return i.material;
		}

		return null;
	}
}
