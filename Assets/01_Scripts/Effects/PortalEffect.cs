using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    private Renderer _targetRenderer;
    [ColorUsage(false, true)][SerializeField] private Color color = Color.white;
    [SerializeField] private float start = 5;
    [SerializeField] private float end = 15;
    [SerializeField] private float sphereIn = 0;
    [SerializeField] private float sphereOut = 1;
    
    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb { get { if (_mpb is null) _mpb = new(); return _mpb; }}

    private void OnEnable()
    {
        GetRenderer();
        UpdateMateiral();
    }

    void GetRenderer()
    {
        _targetRenderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        UpdateMateiral();
    }

    void UpdateMateiral()
    {
        Mpb.SetColor("_Color", color);
        Mpb.SetFloat("_Start", start);
        Mpb.SetFloat("_End", end);
        Mpb.SetVector("_SphereInOut", new Vector2(sphereIn, sphereOut));
        _targetRenderer.SetPropertyBlock(Mpb);
    }
    
    #if UNITY_EDITOR

    private void OnValidate()
    {
        if(_targetRenderer == null) GetRenderer();
        UpdateMateiral();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, start);
        Gizmos.DrawWireSphere(transform.position, end);
    }

#endif
}
