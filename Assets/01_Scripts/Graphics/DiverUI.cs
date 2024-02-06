using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiverUI : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TeamColorSettings teamColors;
    [SerializeField] private Image bottomImage;
    [SerializeField] private Image leftTeam;
    [SerializeField] private Image rightTeam;

    [Header("Override Properties")]
    [Range(0, 1)] public float oxygen = 0.5f;
    [SerializeField] private Color oxygenColor = new Color(0.3f, 0.6f, 1);
    
    private Gauge _oxygenFillGauge = new(30, 1);
    private float _previousOxygen;
    
    private RuntimeMaterial _bottomOverride;
    private PlayerOxygen _playerOxygen;
    
    public static DiverUI Me { get; private set; }

    #region MaterialPropertyIDs

    private readonly int ID_Oxygen = Shader.PropertyToID("_Oxygen");
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
        
        _bottomOverride = RuntimeMaterial.FromImage(bottomImage);
    }

    private void OnDisable()
    {
        bottomImage.material = _bottomOverride.Release();
    }

    void Update()
    {
		if (_playerOxygen == null)
			_playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();

        oxygen = _playerOxygen.CurrentOxygenPercent / 100;
        _oxygenFillGauge.Update(_previousOxygen < oxygen);
        _previousOxygen = oxygen;

        leftTeam.color = teamColors.GetTeamColor(0);
        rightTeam.color = teamColors.GetTeamColor(1);
        _bottomOverride.Mat.SetFloat(ID_Oxygen, oxygen);
        
        _bottomOverride.Mat.SetColor(ID_OxygenColor, oxygenColor * _oxygenFillGauge.FillAmount * 0.5f);

    }
}
