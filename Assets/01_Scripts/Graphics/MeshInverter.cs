using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MeshInverter))]
[CanEditMultipleObjects]
public class MeshInverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        if (GUILayout.Button("Save Asset and Replace"))
        {
            MeshInverter component = target as MeshInverter;
            if (component is not null) component.InvertMesh();
        }
        DrawDefaultInspector();
    }
}
#endif

public class MeshInverter : MonoBehaviour
{
    
    [SerializeField] private string assetName;
    
    #if UNITY_EDITOR
    public void InvertMesh()
    {
        if (assetName == "")
        {
            Debug.LogError("Invalid Asset Name");
            return;
        }
        var filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        Mesh newMesh = new();
        newMesh.vertices = mesh.vertices;
        newMesh.triangles = mesh.triangles.Reverse().ToArray();
        newMesh.uv = mesh.uv;
        newMesh.uv2 = mesh.uv2;
        newMesh.RecalculateNormals();
        AssetDatabase.CreateAsset(newMesh, $"Assets/02_Art/Generated/MDL_{assetName}.asset");
        filter.sharedMesh = newMesh;
    }
    #endif
}
