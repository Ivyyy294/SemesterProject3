using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class TransformDelay : MonoBehaviour
{
    [SerializeField] private float positionSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float directionUpdateMargin = 0.01f;

    [SerializeField] private bool showDebug = true;
    private Vector3 _delayPosition;
    private Quaternion _delayRotation;
    private Vector3 _direction = Vector3.up;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 DelayPosition => _delayPosition;
    public Quaternion DelayRotation => _delayRotation;
    public Vector3 Direction => _direction;

	void OnEnable()
    {
        _delayPosition = Position;
        Application.targetFrameRate = -1;
        
    }
    
    void Update()
    {
        float positionBlend = 1 - Mathf.Pow(0.5f, Time.deltaTime * positionSpeed);
        float rotationBlend = 1 - Mathf.Pow(0.5f, Time.deltaTime * rotationSpeed);
        _delayPosition = Vector3.Lerp(_delayPosition, Position, positionBlend);
        _delayRotation = Quaternion.Slerp(_delayRotation, Rotation, rotationBlend);
        
        
        var delta = _delayPosition - Position;
        if (delta.magnitude > directionUpdateMargin)
        {
            _direction = delta.normalized;
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Position, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(DelayPosition, 0.1f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Position, Position + Direction);
    }
#endif
}
