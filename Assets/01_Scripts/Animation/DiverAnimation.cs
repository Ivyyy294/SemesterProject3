using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InverseChain))]
public class DiverAnimation : MonoBehaviour
{
    [SerializeField] private TransformDelay legDelay;
    [SerializeField] private InverseChain hipSpineChain;
    [SerializeField] private Transform shoulders;
    [SerializeField] private List<Transform> lowerLegs;

    [SerializeField] private Animator animator;

     private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
     private readonly int ID_Levelness = Animator.StringToHash("Levelness");

     private List<Vector3> _previousVelocities;
     private Vector3 _previousPosition;
     private int _positionBufferSize = 24; 
     private Vector3 _velocity;

     private AnimUtils.AngleTracker _angleTracker;

     private void OnEnable()
    {
        _previousVelocities = new List<Vector3>();
        for (int i = 0; i < _positionBufferSize; i++)
        {
            _previousVelocities.Add(transform.position);
        }
    }

     void Update()
    {
        UpdatePreviosPositions(); // Always update first to validate the values
        
        animator.SetFloat(ID_SwimSpeed, Mathf.Clamp01(_velocity.magnitude));
        animator.SetFloat(ID_Levelness, Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)));
    }

    private void LateUpdate()
    {
        UpdateTorsoTransforms();
        UpdateTwistTransforms();
    }

    void UpdatePreviosPositions()
    {
        float xSum = _previousVelocities.Sum(v => v.x);
        float ySum = _previousVelocities.Sum(v => v.y);
        float zSum = _previousVelocities.Sum(v => v.z);
        _velocity = new Vector3(xSum, ySum, zSum) / _previousVelocities.Count;
        _previousVelocities.RemoveAt(0);
        _previousVelocities.Add((transform.position - _previousPosition) / Time.deltaTime);
        _previousPosition = transform.position;

        _angleTracker = new(transform.position, legDelay.DelayPosition, -transform.forward, transform.right, transform.up);
        Debug.Log($"Vertical: {_angleTracker.Angle1}, Horizontal: {_angleTracker.Angle2}");
    }
    
    void UpdateTwistTransforms()
    {
        shoulders.Rotate(Vector3.up, _angleTracker.Angle2 * 0.5f);
    }

    void UpdateTorsoTransforms()
    {
        hipSpineChain.GetOriginal();
        var root = hipSpineChain.RootInverse;
        root.Rotate(Vector3.right, _angleTracker.Angle1 * 1f);
        root.Rotate(Vector3.forward, -_angleTracker.Angle2);
        hipSpineChain.Apply();
        foreach (var t in lowerLegs)
        {
            t.Rotate(Vector3.right, -_angleTracker.Angle1 * 1f);
            Debug.Log(t.localEulerAngles.x);
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if(_angleTracker is not null) _angleTracker.DrawDebug();
    }
    #endif
}
