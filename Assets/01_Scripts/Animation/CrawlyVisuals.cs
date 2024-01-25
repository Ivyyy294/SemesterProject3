using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlyVisuals : MonoBehaviour
{
    [Range(0, 1)] public float spherize = 0f;

    private Renderer[] _renderers;

    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb { get { if (_mpb is null) _mpb = new(); return _mpb; }}
    
    #region ShaderProperties
    private readonly int ID_Spherize = Shader.PropertyToID("_Spherize");
    private readonly int ID_SpherizePosition = Shader.PropertyToID("_SpherizePosition");
    #endregion
    
    private void OnEnable()
    {
        
        var allRenderers = GetComponentsInChildren<Renderer>();
        List<Renderer> selectedRenderers = new();
        foreach (var r in allRenderers)
        {
            if(r.sharedMaterials[0].shader.name.EndsWith("SHG_CrawlyBall")) selectedRenderers.Add(r);
        }
        _renderers = selectedRenderers.ToArray();
        UpdateMaterial();
    }

    private void Update()
    {
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        Mpb.SetFloat(ID_Spherize, Mathf.Clamp01(spherize));
        Mpb.SetVector(ID_SpherizePosition, transform.position);
        
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        foreach (var r in _renderers)
        {
            r.SetPropertyBlock(Mpb);
        }
    }
}
