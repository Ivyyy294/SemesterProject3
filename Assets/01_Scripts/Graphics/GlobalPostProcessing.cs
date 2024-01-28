using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Rendering.Universal;
using System.Reflection;

public class GlobalPostProcessing : MonoBehaviour
{
    [Range(0, 1)] public float speedLines;

    private static GlobalPostProcessing _me;
    public static GlobalPostProcessing Me
    {
        get
        {
            if (_me == null)
            {
                var newObject = new GameObject("GlobalPostProcessing").AddComponent<GlobalPostProcessing>();
                _me = newObject;
            }
            return _me;
        }
    }
    
    private FullScreenPassRendererFeature _feature;
    private Material _originalMaterial;
    private Material _tempMaterial;
    
    #region MaterialParameters
    
    private readonly int ID_NoiseTile = Shader.PropertyToID("_NoiseTile");
    private readonly int ID_Intensity = Shader.PropertyToID("_Intensity");
    private readonly int ID_Speed = Shader.PropertyToID("_Speed");
    private readonly int ID_MaskCenter = Shader.PropertyToID("_MaskCenter");
    private readonly int ID_SpeedLines = Shader.PropertyToID("_SpeedLines");
    
    #endregion

    private void Awake()
    {
        if (_me == null)
        {
            _me = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        // stupid hack to retrieve the current rendererData and with it the rendererFeature
        // that controls the custom post-processing 
        var universalRenderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetRenderer(0);
        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
        List<ScriptableRendererFeature> rendererFeatures = property.GetValue(universalRenderer) as List<ScriptableRendererFeature>;
        
        var feature = rendererFeatures?.OfType<FullScreenPassRendererFeature>()
            .FirstOrDefault(x => x.name.Contains("Wobble"));
        if (feature is null) return;
        
        _feature = feature;
        _originalMaterial = _feature.passMaterial;
        _tempMaterial = new Material(_originalMaterial);
        _tempMaterial.hideFlags = HideFlags.HideAndDontSave;
        _feature.passMaterial = _tempMaterial;
    }

    private void OnDisable()
    {
        if (_feature is null) return;
        _feature.passMaterial = _originalMaterial;
        Destroy(_tempMaterial);
    }

    private void Update()
    {
        _tempMaterial.SetFloat(ID_SpeedLines, speedLines);
    }

    public void ResetEffects()
    {
        speedLines = 0;
    }
}
