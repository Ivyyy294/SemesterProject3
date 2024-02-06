using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSandVFX : MonoBehaviour
{
    [SerializeField] private PhysicMaterial sandMaterial;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private ParticleSystem particles;
    
    private RaycastHit _rayCast;
    private bool _isHit;
    private bool _hasHitSand;

    private void Update()
    {
        _isHit = false;
        _hasHitSand = false;
        CheckRayHit();
        if (_hasHitSand) OnHitSand();
        else
        {
            particles.Stop();
        }
    }

    private void CheckRayHit()
    {
        var ray = new Ray(transform.position, Vector3.down);
        _isHit = Physics.Raycast(ray, out _rayCast, interactionDistance);
        if (!_isHit) return;
        var hitMaterial = _rayCast.collider.sharedMaterial;
        if (hitMaterial == null) return;
        _hasHitSand = hitMaterial.Equals(sandMaterial);
    }
    private void OnHitSand()
    {
        particles.transform.position = _rayCast.point;
        if(!particles.isPlaying) particles.Play();
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _hasHitSand ? Color.green : Color.red;
        var endPoint = _isHit ? _rayCast.point : transform.position + Vector3.down * interactionDistance;
        Gizmos.DrawLine(transform.position, endPoint);
        Gizmos.DrawSphere(endPoint, 0.1f);
    }
#endif
}
