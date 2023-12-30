using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class AssetManager
{
    public static string ArtPath => "Assets/02_Art";
    public static string AnimationPath => $"{ArtPath}/Animation";
    public static string GeneratedPath => $"{ArtPath}/Generated";
    public static string MaterialsPath => $"{ArtPath}/Materials";
    public static string ModelsPath => $"{ArtPath}/Models";
    public static string RigsPath => $"{ArtPath}/Rigs";
    public static string ShadersPath => $"{ArtPath}/Shaders";
    public static string TexturesPath => $"{ArtPath}/Textures";
    public static string ProceduralTexturesPath => $"{TexturesPath}/Procedural";

    public static string Join(params string[] parts)
    {
        return string.Join('/', parts);
    }

    public static T LoadAsset<T>(string path) where T: Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    
    public static ComputeShader GetComputeShader(string name)
    {
        string path = Join(ShadersPath, $"{name}.compute");
        return LoadAsset<ComputeShader>(path);
    }
}
