using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class VelocityTracker
{
    private int _positionBufferSize; 
    
    private List<Vector3> _previousVelocities;
    private Vector3 _previousPosition;
    
    private Vector3 _velocity;
    
    public Vector3 SmoothVelocity => _velocity;
    public float SmoothSpeed => _velocity.magnitude;

    public VelocityTracker(int positionBufferSize = 24)
    {
        _positionBufferSize = positionBufferSize;
        _previousVelocities = new List<Vector3>();
        for (int i = 0; i < _positionBufferSize; i++)
        {
            _previousVelocities.Add(Vector3.zero);
        }
        
        _velocity = Vector3.zero;
    }

    public void FixedUpdate(Vector3 currentPosition)
    {
        Vector3 result = Vector3.zero;
        foreach (Vector3 vel in _previousVelocities)
        {
            result += vel;
        }
        _velocity = result / _previousVelocities.Count;
        _previousVelocities.RemoveAt(0);
        _previousVelocities.Add((currentPosition - _previousPosition) / Time.deltaTime);
        _previousPosition = currentPosition;
    }
}
