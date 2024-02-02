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

    #region FeaturesAndMaterials

    private FullScreenPassRendererFeature _wobbleFeature;
    private RuntimeMaterial _wobbleMaterial;

    private FullScreenPassRendererFeature _transitionFeature;
    private RuntimeMaterial _transitionMaterial;

    #endregion

    #region MaterialParameters
    
    private readonly int ID_NoiseTile = Shader.PropertyToID("_NoiseTile");
    private readonly int ID_Intensity = Shader.PropertyToID("_Intensity");
    private readonly int ID_Speed = Shader.PropertyToID("_Speed");
    private readonly int ID_MaskCenter = Shader.PropertyToID("_MaskCenter");
    private readonly int ID_SpeedLines = Shader.PropertyToID("_SpeedLines");
    private readonly int ID_Transition = Shader.PropertyToID("_Transition");
    
    #endregion

    private Timer _transition = new(2);

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
        if (feature is not null)
        {
            _wobbleFeature = feature;
            _wobbleMaterial = new(_wobbleFeature.passMaterial);
            _wobbleFeature.passMaterial = _wobbleMaterial.Mat;
        }

        feature = rendererFeatures?.OfType<FullScreenPassRendererFeature>().First(x => x.name.Contains("Transition"));
        if (feature is not null)
        {
            _transitionFeature = feature;
            _transitionMaterial = new(_transitionFeature.passMaterial);
            _transitionFeature.passMaterial = _transitionMaterial.Mat;
        }


    }

    private void OnDisable()
    {
        if (_wobbleFeature is null) return;
        _wobbleFeature.passMaterial = _wobbleMaterial.Release();
        _transitionFeature.passMaterial = _transitionMaterial.Release();
    }

    private void Update()
    {
        _transition.Update();
        _wobbleMaterial.Mat.SetFloat(ID_SpeedLines, speedLines);
        _transitionMaterial.Mat.SetFloat(ID_Transition, _transition.ProgressNormalized);
    }

    public void Transition()
    {
        _transition.Start();
    }

    public void ResetEffects()
    {
        speedLines = 0;
    }
}
