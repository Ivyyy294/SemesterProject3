using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct GeoInfo
{
    public Mesh mesh;
    public Material material;

    public GeoInfo(Mesh mesh, Material material)
    {
        this.mesh = mesh;
        this.material = material;
    }

    public GeoInfo(GameObject obj)
    {
        mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        material = obj.GetComponent<Renderer>().sharedMaterial;
    }
}

public class ComputeTest : MonoBehaviour
{
    [SerializeField] private int sphereAmount = 17;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private GameObject prefab;

    private ComputeBuffer _resultBuffer;
    private int _kernel;
    private uint _threadGroupSize;
    private Vector3[] _output;
    private Transform[] _instances;

    private GeoInfo _geoInfo;

    void Start()
    {
        _kernel = computeShader.FindKernel("Spheres");
        computeShader.GetKernelThreadGroupSizes(_kernel, out _threadGroupSize, out _, out _);
        _resultBuffer = new ComputeBuffer(sphereAmount, sizeof(float) * 3);
        _output = new Vector3[sphereAmount];
        _instances = new Transform[sphereAmount];
        _geoInfo = new GeoInfo(prefab);
    }

    void Update()
    {
        computeShader.SetFloat("Time", Time.time);
        computeShader.SetBuffer(_kernel, "Result", _resultBuffer);
        int threadGroups = (int)((sphereAmount + (_threadGroupSize - 1)) / _threadGroupSize);
        computeShader.Dispatch(_kernel, threadGroups, 1, 1);
        _resultBuffer.GetData(_output);
        var matrices = _output.Select(x => Matrix4x4.TRS(x, Quaternion.identity, Vector3.one)).ToArray();
        
        Graphics.RenderMeshInstanced(new RenderParams(_geoInfo.material), _geoInfo.mesh, 0, matrices);
    }

    private void OnDestroy()
    {
        _resultBuffer.Dispose();
    }
}
