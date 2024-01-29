using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DiverUI : MonoBehaviour
{
    [SerializeField] private TeamColorSettings teamColors;
    [Range(0, 1)] public float oxygen = 0.5f;
    [SerializeField] private Color oxygenColor = new Color(0.3f, 0.6f, 1);
    
    private Image _image;
    private Gauge _oxygenFillGauge = new(30, 1);
    private float _previousOxygen;
    
    private Material _tempMaterial;
    private Material _originalMaterial;
    
    public static DiverUI Me { get; private set; }

    #region MaterialPropertyIDs

    private readonly int ID_Slider = Shader.PropertyToID("_Slider");
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
        _image = GetComponent<Image>();
        _oxygenFillGauge.SetFillAmount(0);
        _previousOxygen = oxygen;
        _originalMaterial = _image.material;
        _tempMaterial = new Material(_originalMaterial);
        _tempMaterial.hideFlags = HideFlags.HideAndDontSave;
        _tempMaterial.name = $"{_originalMaterial.name}_Runtime";
        _image.material = _tempMaterial;
    }

    private void OnDisable()
    {
        _image.material = _originalMaterial;
        Destroy(_tempMaterial);
    }

    void Update()
    {
        _oxygenFillGauge.Update(_previousOxygen < oxygen);
        _previousOxygen = oxygen;
        
        _tempMaterial.SetColor(ID_Team1, teamColors.GetTeamColor(0));
        _tempMaterial.SetColor(ID_Team2, teamColors.GetTeamColor(1));
        _tempMaterial.SetFloat(ID_Slider, oxygen);
        var finalColor = Color.Lerp(oxygenColor, Color.white, _oxygenFillGauge.FillAmount * 0.5f);
        _tempMaterial.SetColor(ID_OxygenColor, finalColor);

    }
}
