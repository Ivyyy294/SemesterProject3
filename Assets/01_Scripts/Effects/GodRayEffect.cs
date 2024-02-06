using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRayEffect : MonoBehaviour
{
    private LineRenderer _targetRenderer;
    [ColorUsage(false, true)][SerializeField] private Color color = Color.white;
    [SerializeField] private float thickness = 1;

    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb { get { if (_mpb is null) _mpb = new(); return _mpb; }}

    private void OnEnable()
    {
        GetRenderer();
        UpdateMateiral();
    }

    void GetRenderer()
    {
        _targetRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateMateiral();
    }

    void UpdateMateiral()
    {
        Mpb.SetColor("_Color", color);
        _targetRenderer.SetPropertyBlock(Mpb);
        _targetRenderer.widthMultiplier = thickness;
    }
    
    #if UNITY_EDITOR

    private void OnValidate()
    {
        if(_targetRenderer == null) GetRenderer();
        UpdateMateiral();
    }

#endif
}
