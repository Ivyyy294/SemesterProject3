using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DiverAnimation : MonoBehaviour
{
    [Header("Local Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] private DiverVerletBehavior verletBehavior;

    [Header("External Dependencies")]
    [SerializeField] private PlayerBallStatus playerBallStatus;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform ball;
    
    [Header("Joints References")]
    [SerializeField] private MultiInverseChain spineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;

    #region AnimatorParameterIDs
    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_Levelness = Animator.StringToHash("Levelness");
    private readonly int ID_BreastStrokeUB = Animator.StringToHash("BreastStrokeUB");
    private readonly int ID_CancelAnimEffects = Animator.StringToHash("CancelAnimEffects");
    private readonly int ID_IsBlocking = Animator.StringToHash("IsBlocking");
    private readonly int ID_BallState = Animator.StringToHash("BallState");
    private readonly int ID_BallAnglePitch = Animator.StringToHash("BallAnglePitch");
    private readonly int ID_BallAngleYaw = Animator.StringToHash("BallAngleYaw");
    #endregion
    
     private VelocityTracker _velocityTracker;
     
     [Header("Animation Timing")]

     [SerializeField] private Cooldown breastStrokeCooldown;
     [SerializeField] private float lowSpeedThreshold;

     private AnimUtils.AngleTracker _inertiaTracker;
     private AnimUtils.AngleTracker _ballTracker;
     private bool _isHoldingBall;

     private StateListener<PlayerBallStatus, bool> _hasBallListener;
     private StateListener<PlayerBlock, bool> _isBlockingListener;
     private Gauge _dashingGauge;
     

     private void OnEnable()
    {
        verletBehavior.ResetSimulation();
        
        _isHoldingBall = false;
        _hasBallListener = new(playerBallStatus, x => x.HasBall(), _isHoldingBall, SetHoldingBall);
        _isBlockingListener = new(playerBlock, x => x.IsBlocking, false, SetBlocking);
        _velocityTracker = new VelocityTracker(transform.position, 24);
        
        breastStrokeCooldown.MakeReady();
        _dashingGauge = new Gauge(fillRate: 2, depletionRate: 3);
        
        animator.SetFloat(ID_SwimSpeed, 0);
    }

     void Update()
     {
         _hasBallListener.Update();
         _isBlockingListener.Update();
         _dashingGauge.Update(playerMovement.IsDashing && _velocityTracker.SmoothSpeed > 2f);
         
         _inertiaTracker = new(
             transform.position, 
             verletBehavior.SmoothTarget, 
             -transform.forward, 
             transform.right,
             transform.up);
         _ballTracker = new(
             upperSpine.position, 
             GetBallTargetPosition(), 
             transform.up, 
             transform.right, 
             transform.forward);

         float velocityDot = Vector3.Dot(transform.forward, _velocityTracker.SmoothVelocity.normalized);

         var goingSlow = _velocityTracker.SmoothSpeed < lowSpeedThreshold;
         var isBlocking = _isBlockingListener.CurrentValue;
         if (goingSlow)
         {
             breastStrokeCooldown.Update();
         }

         if (!goingSlow && !_isHoldingBall && !isBlocking && velocityDot > 0.3f)
         {
             if(breastStrokeCooldown.Trigger()) animator.SetTrigger(ID_BreastStrokeUB);
             else breastStrokeCooldown.Reset();
         }

         animator.SetFloat(ID_Levelness, Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)));
         animator.SetFloat(ID_SwimSpeed, (Mathf.Clamp01(_velocityTracker.SmoothSpeed) + _dashingGauge.FillAmount) * velocityDot);
         animator.SetFloat(ID_BallAnglePitch, _ballTracker.Angle1 / 90f);
         animator.SetFloat(ID_BallAngleYaw, _ballTracker.Angle2 / 90f);
         
         #region TESTING_INPUTS
         if (Input.GetKeyDown(KeyCode.E))
         {
             ToggleHoldingBall();
         }

         if (Input.GetKeyDown(KeyCode.Alpha0))
         {
             animator.SetInteger(ID_BallState, 0);
         }

         if (Input.GetKeyDown(KeyCode.Alpha1))
         {
             animator.SetInteger(ID_BallState, 1);
         }

         if (Input.GetKeyDown(KeyCode.Alpha2))
         {
             animator.SetInteger(ID_BallState, 2);
         }
         #endregion
         
     }

     void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position); // Always update first to validate the values
    }

    private void LateUpdate()
    {
        UpdateSimulatedLimbs();
    }

    public void UpdateSimulatedLimbs()
    {
        // float speedMultiplier = MathfUtils.RemapClamped(_velocityTracker.SmoothSpeed, 1.2f, 3.5f, 1, .5f);
        // float directionDot = Vector3.Dot(transform.forward, _velocityTracker.SmoothVelocity.normalized);
        // float directionMultiplier = MathfUtils.RemapClamped(directionDot, -.4f, 0, 0, 1);
        // float multiplier = speedMultiplier * directionMultiplier;
        float multiplier = 1.5f;

        // Update Torso Transforms

        if (Input.GetKey(KeyCode.T)) return;
        spineChain.GetOriginal();
        for (int i = 0; i < spineChain.ChainLength - 1; i++)
        {
            var t = spineChain.GetInverse(i);
            t.Rotate(Vector3.right, _inertiaTracker.Angle1 * 0.3f * multiplier);
            t.Rotate(Vector3.forward, -_inertiaTracker.Angle2 * 0.3f * multiplier);
        }
        
        spineChain.Apply();
        

        foreach (var t in lowerLegs)
        {
            var vectorPitch = t.InverseTransformDirection(transform.right);
            t.Rotate(vectorPitch, _inertiaTracker.Angle1 * 0.7f * multiplier);
        }
        
        // Update Twist Transforms
        
        float verticalAngle = _inertiaTracker.Angle1 * multiplier;
        float horizontalAngle = _inertiaTracker.Angle2 * multiplier;
        upperSpine.Rotate(Vector3.up, horizontalAngle * 0.5f * multiplier);
        upperSpine.Rotate(Vector3.right, -verticalAngle * 0.5f * multiplier);
        foreach (var t in arms)
        {
            var vectorYaw = t.InverseTransformDirection(transform.up);
            var vectorPitch = t.InverseTransformDirection(transform.right);
            t.Rotate(vectorYaw, horizontalAngle * 0.5f * multiplier);
            t.Rotate(vectorPitch, verticalAngle * 0.7f * multiplier);
        }
    }

    public void SetHoldingBall(bool newValue)
    {
        if (_isHoldingBall == newValue) return;
        _isHoldingBall = newValue;
        
        animator.SetInteger(ID_BallState, _isHoldingBall? 2 : 0);
        animator.SetTrigger(ID_CancelAnimEffects);
    }

    public void ToggleHoldingBall()
    {
        SetHoldingBall(!_isHoldingBall);
    }

    public void SetBlocking(bool isBlocking)
    {
        if (isBlocking == animator.GetBool(ID_IsBlocking)) return;
        animator.SetBool(ID_IsBlocking, isBlocking);
        animator.SetTrigger(ID_CancelAnimEffects);
    }

    public void ResetSimulations()
    {
        _velocityTracker.ResetVelocities(transform.position);
        animator.SetTrigger(ID_CancelAnimEffects);
        verletBehavior.ResetSimulation();
    }

    private Vector3 GetBallTargetPosition()
    {
        // TODO: Stable reference to ball through outside script
        if (ball == null)
        {
            return transform.position + transform.forward * 3;
        }
        return ball.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if(_angleTracker is not null) _angleTracker.DrawDebug();
        // if(_ballTracker is not null) _ballTracker.DrawDebug();
        // spineChain.DrawGizmos(0.05f);
    }
    #endif
}
