using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InverseChain))]
public class DiverAnimation : MonoBehaviour
{
    [SerializeField] private TransformDelay legDelay;
    [SerializeField] private InverseChain hipSpineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;

    [SerializeField] private Animator animator;

    [SerializeField] private float fastTravelSpeedThreshold;

     private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
     private readonly int ID_Levelness = Animator.StringToHash("Levelness");
     private readonly int ID_IsHoldingBall = Animator.StringToHash("IsHoldingBall");
     private readonly int ID_IsMovingFastOverTime = Animator.StringToHash("IsMovingFastOverTime");

     private List<Vector3> _previousVelocities;
     private Vector3 _previousPosition;
     private int _positionBufferSize = 24; 
     private Vector3 _velocity;
     private float _fastTravelTimer;

     private AnimUtils.AngleTracker _angleTracker;
     private bool _isHoldingBall;

     private void OnEnable()
    {
        _previousVelocities = new List<Vector3>();
        for (int i = 0; i < _positionBufferSize; i++)
        {
            _previousVelocities.Add(transform.position);
        }

        _isHoldingBall = false;
        _fastTravelTimer = 0;

		UpdatePreviosPositions();
    }

     void Update()
     {
         if (Input.GetKeyDown(KeyCode.E))
         {
             _isHoldingBall = !_isHoldingBall;
             animator.SetBool(ID_IsHoldingBall, _isHoldingBall);
         }
     }

     void FixedUpdate()
    {
        UpdatePreviosPositions(); // Always update first to validate the values

        if (_velocity.magnitude > fastTravelSpeedThreshold)
        {
            _fastTravelTimer += Time.deltaTime;
        }
        else _fastTravelTimer = 0;
        
        animator.SetFloat(ID_SwimSpeed, Mathf.Clamp01(_velocity.magnitude));
        animator.SetFloat(ID_Levelness, Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)));
        animator.SetBool(ID_IsMovingFastOverTime, _fastTravelTimer > 1f && !_isHoldingBall);
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
    }
    
    void UpdateTwistTransforms()
    {
        float verticalAngle = _angleTracker.Angle1;
        float horizontalAngle = _angleTracker.Angle2;
        upperSpine.Rotate(Vector3.up, horizontalAngle * 0.5f);
        upperSpine.Rotate(Vector3.right, -verticalAngle * 0.5f);
        foreach (var t in arms)
        {
            var vectorYaw = t.InverseTransformDirection(transform.up);
            var vectorPitch = t.InverseTransformDirection(transform.right);
            t.Rotate(vectorYaw, horizontalAngle * 0.5f);
            t.Rotate(vectorPitch, verticalAngle * 0.5f);
        }
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
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if(_angleTracker is not null) _angleTracker.DrawDebug();
    }
    #endif
}
