using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TeamColorSettings teamColors;
    [SerializeField] private Image topImage;
    [SerializeField] private Image bottomImage;

    [Header("Override Properties")]
    [Range(0, 1)] public float oxygen = 0.5f;
    [SerializeField] private Color oxygenColor = new Color(0.3f, 0.6f, 1);
    
    private Gauge _oxygenFillGauge = new(30, 1);
    private float _previousOxygen;

    private RuntimeMaterial _topOverride;
    private RuntimeMaterial _bottomOverride;
    
    public static DiverUI Me { get; private set; }

    #region MaterialPropertyIDs

    private readonly int ID_Oxygen = Shader.PropertyToID("_Oxygen");
    private readonly int ID_Team1 = Shader.PropertyToID("_Team1");
    private readonly int ID_Team2 = Shader.PropertyToID("_Team2");
    private readonly int ID_OxygenColor = Shader.PropertyToID("_OxygenColor");

    #endregion

    private void Awake()
    {
        if (Me == null) Me = this;
        else Destroy(this);
    }

    private void OnEnable()
    {
        _oxygenFillGauge.SetFillAmount(0);
        _previousOxygen = oxygen;
        
        _topOverride = RuntimeMaterial.FromImage(topImage);
        _bottomOverride = RuntimeMaterial.FromImage(bottomImage);
    }

    private void OnDisable()
    {
        topImage.material = _topOverride.Release();
        bottomImage.material = _bottomOverride.Release();
    }

    void Update()
    {
        _oxygenFillGauge.Update(_previousOxygen < oxygen);
        _previousOxygen = oxygen;
        
        _topOverride.Mat.SetColor(ID_Team1, teamColors.GetTeamColor(0));
        _topOverride.Mat.SetColor(ID_Team2, teamColors.GetTeamColor(1));
        _bottomOverride.Mat.SetFloat(ID_Oxygen, oxygen);
        
        var finalColor = Color.Lerp(oxygenColor, Color.white, _oxygenFillGauge.FillAmount * 0.5f);
        _bottomOverride.Mat.SetColor(ID_OxygenColor, finalColor);

    }
}
