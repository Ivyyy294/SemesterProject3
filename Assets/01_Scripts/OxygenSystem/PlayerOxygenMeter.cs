using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerOxygen))]
public class PlayerOxygenMeter : MonoBehaviour
{
	public Gradient oxygenLevelColors;

	private MaterialPropertyBlock _mpb;
	public MaterialPropertyBlock Mpb { get {
			if (_mpb is null) _mpb = new MaterialPropertyBlock();
			return _mpb; } }
	
	[Header ("Lara Values")]
	[SerializeField] Renderer OxygenMeterRenderer;

	PlayerOxygen playerOxygen;

	// Start is called before the first frame update
    void Start()
    {
        playerOxygen = GetComponent<PlayerOxygen>();
    }

    // Update is called once per frame
    void Update()
    {
	    Mpb.SetColor("_BaseColor", oxygenLevelColors.Evaluate(playerOxygen.CurrentOxygenPercent / 100f));
	    OxygenMeterRenderer.SetPropertyBlock(Mpb);
    }
}
