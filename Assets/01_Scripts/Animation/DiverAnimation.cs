using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InverseChain))]
public class DiverAnimation : MonoBehaviour
{
    [SerializeField] private TransformDelay legDelay;
    [SerializeField] private InverseChain hipSpineChain;

    [SerializeField] private Animator animator;

     private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
     private readonly int ID_Levelness = Animator.StringToHash("Levelness");

     private List<Vector3> _previousVelocities;
     private Vector3 _previousPosition;
     private int _positionBufferSize = 24; 
     private Vector3 _velocity;

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
    }
    
    void UpdateTwistTransforms()
    {
        // upperBody.localEulerAngles = new Vector3(0, 0, -diverInput.YawSway * 30f);
    }

    void UpdateTorsoTransforms()
    {
        var direction = legDelay.DelayPosition - hipSpineChain.root.position;
        var upDirection = legDelay.DelayRotation * Vector3.up;
        var targetRotation = Quaternion.LookRotation(direction, upDirection);
        hipSpineChain.GetOriginal();
        var root = hipSpineChain.RootInverse;
        root.rotation = targetRotation;
        root.Rotate(Vector3.forward, 180);
        root.Rotate(Vector3.right, -90);
        hipSpineChain.Apply();
        
        
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }
    #endif
}
