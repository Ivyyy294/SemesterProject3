using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class VelocityTracker
{
    private int _positionBufferSize; 
    
    private Queue<Vector3> _previousVelocities;
    private Vector3 _previousPosition;
    
    private Vector3 _velocity;
    
    public Vector3 SmoothVelocity => _velocity;
    public float SmoothSpeed => _velocity.magnitude;

    public VelocityTracker(Vector3 position, int positionBufferSize = 24)
    {
        _positionBufferSize = positionBufferSize;
        ResetVelocities(position);
    }

    public void FixedUpdate(Vector3 currentPosition)
    {
        Vector3 result = Vector3.zero;
        foreach (Vector3 vel in _previousVelocities)
        {
            result += vel;
        }
        _velocity = result / _previousVelocities.Count;
        _previousVelocities.Dequeue();
        _previousVelocities.Enqueue((currentPosition - _previousPosition) / Time.fixedDeltaTime);
        _previousPosition = currentPosition;
    }

    public void ResetVelocities(Vector3 position)
    {
        _velocity = Vector3.zero;
        _previousPosition = position;
        _previousVelocities = new Queue<Vector3>();
        for (int i = 0; i < _positionBufferSize; i++)
        {
            _previousVelocities.Enqueue(Vector3.zero);
        }
    }
}
