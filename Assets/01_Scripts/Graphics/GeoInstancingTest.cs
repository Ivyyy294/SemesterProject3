using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoInstancingTest : MonoBehaviour
{
    public Transform prefab;
    public int instances = 5000;
    public float radius = 50f;

    private Mesh _mesh;
    private Material _material;
    private Matrix4x4[] _matrices;

    void Start()
    {
        // SpawnSphereGameObjects();
        SetupInstancedDraw();
        
    }

    void Update()
    {
        DrawInstanced(_mesh, _material, _matrices);
    }

    void SpawnSphereGameObjects()
    {
        MaterialPropertyBlock properties = new();
        for (int i = 0; i < instances; i++)
        {
            Transform t = Instantiate(prefab, transform);
            t.localPosition = radius * Random.insideUnitSphere;
            
            properties.SetColor("_Color", new Color(Random.value, Random.value, Random.value));
            // t.GetComponent<MeshRenderer>().SetPropertyBlock(properties);
        }
    }

    void SetupInstancedDraw()
    {
        _mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        _material = prefab.GetComponent<Renderer>().sharedMaterial;
        _matrices = new Matrix4x4[instances];
        for (int i = 0; i < instances; i++)
        {
            _matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * radius, Quaternion.identity, Vector3.one);
        }
    }
    
    void DrawInstanced(Mesh mesh, Material material, Matrix4x4[] matrices)
    {
        RenderParams rparams = new RenderParams(material);   
        Graphics.RenderMeshInstanced(rparams, mesh, 0, matrices);
    }
}
