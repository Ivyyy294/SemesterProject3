using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverOxygenAnimation : MonoBehaviour
{
    [SerializeField] private DiverVisuals diverVisuals;
    [SerializeField] private PlayerOxygen playerOxygen;
    [SerializeField] private TeamColor teamColor;

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
        diverVisuals.emissiveColor = teamColor.Color;
        diverVisuals.oxygenLevel = _oxygen;
    }

    private float GetOxygenState()
    {
        return playerOxygen.CurrentOxygenPercent / 100;
    }
}
