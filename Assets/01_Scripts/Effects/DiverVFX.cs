using System;
using System.Collections;
using System.Collections.Generic;
using Ivyyy.Network;
using UnityEngine;

public class DiverVFX : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private NetworkBehaviour _networkPlayer;
    
    [Header("Local References")]
    [SerializeField] private TrailEffect trailPrefab;
    [SerializeField] private List<Transform> trailParents;
    [SerializeField] private ParticleSystem bubbleParticles;
    public Color color;
    public int InstanceCount => trailParents.Count;
    
    private TrailEffect[] _effectInstances;
    private VelocityTracker _velocityTracker;
    private GlobalPostProcessing _globalPP;
    
    private void OnEnable()
    {
        _effectInstances = new TrailEffect[InstanceCount];
        for (int i = 0; i < InstanceCount; i++)
        {
            _effectInstances[i] = Instantiate(trailPrefab, trailParents[i]);
        }
        _velocityTracker = new(transform.position, 20);
        
        _globalPP = GlobalPostProcessing.Me;
    }

    public void ResetTrails()
    {
        _velocityTracker.ResetVelocities(transform.position);
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

        if (_networkPlayer.Owner)
        {
            _globalPP.speedLines = MathfUtils.RemapClamped(_velocityTracker.SmoothSpeed, 2.5f, 4, 0, 1); 
            if(!bubbleParticles.isPlaying) bubbleParticles.Play();
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
