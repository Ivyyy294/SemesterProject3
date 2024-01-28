using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverVFX : MonoBehaviour
{
    [SerializeField] private TrailEffect trailPrefab;
    [SerializeField] private List<Transform> trailParents;
    public Color color;
    public int InstanceCount => trailParents.Count;
    
    private TrailEffect[] _effectInstances;
    private VelocityTracker _velocityTracker;
    
    private void OnEnable()
    {
        _effectInstances = new TrailEffect[InstanceCount];
        for (int i = 0; i < InstanceCount; i++)
        {
            _effectInstances[i] = Instantiate(trailPrefab, trailParents[i]);
        }
        _velocityTracker = new(transform.position, 20);
    }

    public void ResetTrails()
    {
        foreach (var t in _effectInstances)
        {
            t.ResetSimulation();
        }
    }

    private void Update()
    {
        var opacity = MathfUtils.Remap(_velocityTracker.SmoothSpeed, 0.5f, 3f, 0, 1);
        foreach (var t in _effectInstances)
        {
            t.color = color * opacity;
        }
    }

    private void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position);
    }

    private void OnDisable()
    {
        foreach (var t in _effectInstances)
        {
            Destroy(t.gameObject);
        }
        _effectInstances = null;
    }
}
