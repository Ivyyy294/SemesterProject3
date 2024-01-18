using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DiverVisuals : MonoBehaviour
{
    [SerializeField] private GameObject rig;
    public Gradient oxygenColorGradient;
    [Range(0, 1)] public float oxygenLevel = 0.5f;

    private Renderer[] _renderers;

    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb { get { if (_mpb is null) _mpb = new(); return _mpb; }}
    
    #region ShaderProperties
    private readonly int ID_OxygenLevel = Shader.PropertyToID("_OxygenLevel");
    private readonly int ID_OxygenColor = Shader.PropertyToID("_OxygenColor");
    #endregion
    
    private void OnEnable()
    {
        
        var allRenderers = GetComponentsInChildren<Renderer>();
        List<Renderer> selectedRenderers = new();
        foreach (var r in allRenderers)
        {
            if(r.sharedMaterials[0].shader.name.EndsWith("SHG_Diver")) selectedRenderers.Add(r);
        }
        _renderers = selectedRenderers.ToArray();
        UpdateMaterial();
    }

    public void Update()
    {
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        Color sample = oxygenColorGradient.Evaluate(oxygenLevel);
        sample *= sample.a * 2;
        Mpb.SetColor(ID_OxygenColor, sample);
        Mpb.SetFloat(ID_OxygenLevel, oxygenLevel);
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        if (_renderers is null) return;
        foreach (var r in _renderers)
        {
            r.SetPropertyBlock(_mpb);
        }
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateMaterial();
    }
    #endif
}

