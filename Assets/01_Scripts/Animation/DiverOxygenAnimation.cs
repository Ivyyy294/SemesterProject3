using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverOxygenAnimation : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private PlayerOxygen playerOxygen;
    [SerializeField] private PlayerConfigurationContainer playerConfig;
    [SerializeField] private TeamColorSettings teamColors;
    
    [Header("Local References")]
    [SerializeField] private DiverVisuals diverVisuals;

    private float _oxygen;
    private Gauge _oxygenGainGauge = new(20, 1.5f);
    private Color _oxygenFillColor = Color.white;

    private void Start()
    {
        _oxygen = 0;
    }

    private void Update()
    {
        bool oxygenIncreased = false;
        float newOxygen = GetOxygenState();
        if (_oxygen < newOxygen)
        {
            oxygenIncreased = true;
        }
        _oxygen = newOxygen;
        _oxygenGainGauge.Update(oxygenIncreased);
        // Color resultColor = Color.Lerp(teamColor.Color, _oxygenFillColor, _oxygenGainGauge.FillAmount * 0.75f);

		if (playerConfig.playerConfiguration && playerConfig.playerConfiguration.lgbtq)
			diverVisuals.emissiveColor = teamColors.CycleRainbowColors();
		else
			diverVisuals.emissiveColor = teamColors.GetTeamColor(playerConfig.TeamIndex);
        
		diverVisuals.oxygenLevel = _oxygen;

        var hasMaterialVariation = PlayerConfigurationListener.Me.LookUpVariation(playerConfig.PlayerID);
        diverVisuals.variationColor = hasMaterialVariation ? Color.white : Color.black;
    }

    private float GetOxygenState()
    {
        return playerOxygen.CurrentOxygenPercent / 100;
    }
}
