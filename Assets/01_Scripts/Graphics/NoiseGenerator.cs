using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public enum NoiseType
{
    White,
    Perlin,
    Voronoi,
    Cell
}

public enum ResolutionPreset
{
    _64,
    _128,
    _256,
    _512,
    _1024,
    _2048
}

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseGenerator generator = target as NoiseGenerator;
        if (generator is null) return;

        if (GUILayout.Button("Generate!"))
        {
            generator.GenerateTexture();
        }
        DrawDefaultInspector();
    }
}


public class NoiseGenerator : MonoBehaviour
{
    public NoiseType noiseType;
    public ResolutionPreset resolutionPreset;
    public TextureFormat textureFormat = TextureFormat.RGB24;
    [Min(1)]public int noiseScale = 4;
    public ComputeShader shader;

    public static int GetResolution(ResolutionPreset r)
    {
        switch (r)
        {
            case ResolutionPreset._64: return 64;
            case ResolutionPreset._128: return 128;
            case ResolutionPreset._256: return 256;
            case ResolutionPreset._512: return 512;
            case ResolutionPreset._1024: return 1024;
            case ResolutionPreset._2048: return 2048;
            default: return 64;
        }
    }

    public static void FillTexture(ComputeShader shader, Texture2D texture, NoiseType noise, int scale)
    {
        int width = texture.width;
        int height = texture.height;
        int kernel = shader.FindKernel(noise.ToString());
        uint tX, tY, tZ;
        shader.GetKernelThreadGroupSizes(kernel, out tX, out tY, out tZ);

        var renderTexture = new RenderTexture(texture.width, texture.height, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        shader.SetTexture(kernel, "Result", renderTexture);
        shader.SetInts("TextureSize", new[]{width, height});
        shader.SetInt("NoiseScale", scale);
        shader.Dispatch(kernel, width/(int)tX, height/(int)tY, 1);

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        renderTexture.Release();
    }
    

    public static Color WhiteNoise(float u, float v)
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    public static Color PerlinNoise(float u, float v, int scale)
    {
        u *= scale;
        v *= scale;
        u = Mathf.Floor(u);
        v = Mathf.Floor(v);
        u /= scale;
        v /= scale;
        return new Color(u, v, 0);
    }

    public void GenerateTexture()
    {
        Debug.Log("Generating");
        int res = GetResolution(resolutionPreset);
        string textureName = $"TEX_{noiseType.ToString()}Noise_{res.ToString()}.asset";
        Texture2D texture = new Texture2D(res, res, textureFormat, mipChain: true, linear: true);
        FillTexture(shader, texture, noiseType, noiseScale);
        AssetDatabase.CreateAsset(texture, $"Assets/02_Art/Textures/Procedural/{textureName}.asset");
    }
}
