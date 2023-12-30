using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public enum NoiseType
{
    White,
    Perlin,
    Fractal,
    Voronoi,
    Cell,
    TestUV
}

public enum ResolutionPreset
{
    _16,
    _32,
    _64,
    _128,
    _256,
    _512,
    _1024,
    _2048
}

#if UNITY_EDITOR
[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseGenerator generator = target as NoiseGenerator;
        if (generator is null) return;

        if (GUILayout.Button("Generate!")) generator.GenerateTexture();
        
        DrawDefaultInspector();
    }
}

public class NoiseGenerator : MonoBehaviour
{
    public NoiseType noiseType;
    public ResolutionPreset resolutionPreset;
    public int Resolution => GetResolution(resolutionPreset);
    public TextureFormat textureFormat = TextureFormat.RGB24;
    [Min(1)]public int noiseScale = 4;
    [Min(1)] public int octaves = 1;
    public bool useLinear = true;
    public bool _3D = false;


    public static int GetResolution(ResolutionPreset r)
    {
        return int.Parse(r.ToString().Replace("_", ""));
    }
    
    public static ComputeShader GetComputeShader()
    {
        return AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/02_Art/Shaders/NoiseGenerator.compute");
    }

    public Texture2D CreateTexture2D()
    {
        return new Texture2D(
            Resolution, 
            Resolution, 
            textureFormat, 
            mipChain: true, 
            linear: useLinear, 
            createUninitialized: true);
    }
    
    public Texture3D CreateTexture3D()
    {
        return new Texture3D(
                Resolution,
                Resolution,
                Resolution,
            textureFormat,
            mipChain: true,
            createUninitialized: true);
    }

    private string GetAssetPath()
    {
        string scaleText = noiseType == NoiseType.White ? "" : noiseScale.ToString();
        string prefixText = _3D ? "VOL" : "TEX";
        string textureName = $"{prefixText}_Noise{noiseType.ToString()}{scaleText}_{Resolution.ToString()}.asset";
        string path = $"Assets/02_Art/Textures/Procedural/{textureName}.asset";
        return path;
    }

    private Texture3D GetTexture3D(string path, out bool assetExists)
    {
        assetExists = true;
        Texture3D asset = AssetDatabase.LoadAssetAtPath<Texture3D>(path);
        if (asset is not null) return asset;
        assetExists = false;
        return CreateTexture3D();
    }

    private Texture2D GetTexture2D(string path, out bool assetExists)
    {
        assetExists = true;
        Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (asset is not null) return asset;
        assetExists = false;
        return CreateTexture2D();
    }
    
    public void GenerateTexture()
    {
        Debug.Log("Generate Texture");
        int res = Resolution;
        string path = GetAssetPath();

        var shader = GetComputeShader();
        string _3DSuffix = _3D ? "3D" : "";
        int kernel = shader.FindKernel($"{noiseType}{_3DSuffix}");
        uint tX, tY, tZ;
        shader.GetKernelThreadGroupSizes(kernel, out tX, out tY, out tZ);

        int pixelCount = (int)Mathf.Pow(res, _3D? 3 : 2);
        var buffer = new ComputeBuffer(pixelCount, sizeof(float) * 4);
        Color[] colors = new Color[pixelCount];
        
        shader.SetBuffer(kernel, "Result", buffer);
        shader.SetInt("TextureSize", res);
        shader.SetInt("NoiseScale", noiseScale);
        shader.SetInt("NoiseOctaves", octaves);
        shader.Dispatch(kernel, res/(int)tX, res/(int)tY, res/(int)tZ);
        
        buffer.GetData(colors);
        buffer.Dispose();

        bool assetExists;
        if (_3D)
        {
            var texture = GetTexture3D(path, out assetExists);
            texture.SetPixels(colors);
            texture.Apply();
            if (assetExists) AssetDatabase.SaveAssets();
            else AssetDatabase.CreateAsset(texture, path);
        }
        else
        {
            var texture = GetTexture2D(path, out assetExists);
            texture.SetPixels(colors);
            texture.Apply();
            if (assetExists) AssetDatabase.SaveAssets();
            else AssetDatabase.CreateAsset(texture, path);
        }
    }
}
#endif

