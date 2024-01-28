using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailEffect : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    
    public int pointCount = 10;
    public int smoothingSampleCount = 3;
    public float width = 1;
    public Color color;
    
    private Queue<Vector3> _linePoints;
    private List<Vector3> _previousPositions;

    private void OnEnable()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        ResetSimulation();
    }

    private void FixedUpdate()
    {
        Vector3 smoothPosition = Vector3.zero;
        
        _previousPositions.RemoveAt(0);
        _previousPositions.Add(transform.position);
        for (int i = 0; i < smoothingSampleCount; i++)
        {
            smoothPosition += _previousPositions[i];
        }
        smoothPosition /= smoothingSampleCount;

        _linePoints.Dequeue();
        _linePoints.Enqueue(smoothPosition);
        
        _lineRenderer.widthMultiplier = width;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.SetPositions(_linePoints.ToArray());
    }
    
    public void ResetSimulation()
    {
        _linePoints = new();
        _previousPositions = new();
        for (int i = 0; i < pointCount; i++)
        {
            _linePoints.Enqueue(transform.position);
        }

        for (int i = 0; i < smoothingSampleCount; i++)
        {
            _previousPositions.Add(transform.position);
        }
        _lineRenderer.positionCount = pointCount;
    }
}
