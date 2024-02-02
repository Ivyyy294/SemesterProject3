using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DiverVisuals : MonoBehaviour
{
    public Color emissiveColor;
    [Range(0, 1)] public float oxygenLevel = 0.5f;
    public Color variationColor;

    private Renderer[] _renderers;

    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb { get { if (_mpb is null) _mpb = new(); return _mpb; }}
    
    #region ShaderProperties
    private readonly int ID_OxygenLevel = Shader.PropertyToID("_OxygenLevel");
    private readonly int ID_OxygenColor = Shader.PropertyToID("_OxygenColor");
    private readonly int ID_MaskColor = Shader.PropertyToID("_MaskColor");
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
        Mpb.SetColor(ID_OxygenColor, emissiveColor);
        Mpb.SetFloat(ID_OxygenLevel, oxygenLevel);
        Mpb.SetColor(ID_MaskColor, variationColor);
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
}

