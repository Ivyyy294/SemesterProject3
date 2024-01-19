using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InverseChain))]
public class DiverAnimation : MonoBehaviour
{
    [Header("Local Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] private TransformDelay legDelay;
    [Header("External Dependencies")]
    [SerializeField] private PlayerBallStatus playerBallStatus;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform ball;
    
    [Header("Joints References")]
    [SerializeField] private InverseChain hipSpineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;

    #region AnimatorParameterIDs
    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_Levelness = Animator.StringToHash("Levelness");
    private readonly int ID_IsMovingFastOverTime = Animator.StringToHash("IsMovingFastOverTime");
    private readonly int ID_BreastStroke = Animator.StringToHash("BreastStrokeUB");
    private readonly int ID_CancelAnimEffects = Animator.StringToHash("CancelAnimEffects");
    private readonly int ID_IsBlocking = Animator.StringToHash("IsBlocking");
    private readonly int ID_BallState = Animator.StringToHash("BallState");
    private readonly int ID_BallAnglePitch = Animator.StringToHash("BallAnglePitch");
    private readonly int ID_BallAngleYaw = Animator.StringToHash("BallAngleYaw");
    #endregion
    
     private VelocityTracker _velocityTracker;
     
     [Header("Animation Timing")]
     [SerializeField] private TimeCounter fastTravelTimer;
     [SerializeField] private float fastTravelSpeedThreshold;

     [SerializeField] private Cooldown breastStrokeCooldown;
     [SerializeField] private float lowSpeedThreshold;

     private AnimUtils.AngleTracker _angleTracker;
     private AnimUtils.AngleTracker _ballTracker;
     private bool _isHoldingBall;

     private StateListener<PlayerBallStatus, bool> _hasBallListener;
     private StateListener<PlayerBlock, bool> _isBlockingListener;
     private Gauge _dashingGauge;

     private void OnEnable()
    {
        animator.SetFloat(ID_SwimSpeed, 0);
        _velocityTracker = new VelocityTracker(transform.position, 24);
        _isHoldingBall = false;
        breastStrokeCooldown.MakeReady();
        _hasBallListener = new(playerBallStatus, x => x.HasBall(), false, SetHoldingBall);
        _isBlockingListener = new(playerBlock, x => x.IsBlocking, false, SetBlocking);
        _dashingGauge = new Gauge(fillRate: 2, depletionRate: 3);
    }

     void Update()
     {
         _hasBallListener.Update();
         _isBlockingListener.Update();
         _dashingGauge.Update(playerMovement.IsDashing && _velocityTracker.SmoothSpeed > 2f);

         _angleTracker = new(transform.position, legDelay.DelayPosition, -transform.forward, transform.right, transform.up);
         
         _ballTracker = new(upperSpine.position, GetBallTargetPosition(), transform.up, transform.right, transform.forward);

         float velocityDot = Vector3.Dot(transform.forward, _velocityTracker.SmoothVelocity.normalized);
         

         if (_velocityTracker.SmoothSpeed > fastTravelSpeedThreshold && velocityDot > 0)
         {
             fastTravelTimer.Update();
         } else fastTravelTimer.Reset();

         if (_velocityTracker.SmoothSpeed < lowSpeedThreshold)
         {
             breastStrokeCooldown.Update();
         }
         else if(!_isHoldingBall && _isBlockingListener.CurrentValue && velocityDot > 0.3f)
         {
             if(breastStrokeCooldown.Trigger()) animator.SetTrigger(ID_BreastStroke);
             breastStrokeCooldown.Reset();
         }

         animator.SetFloat(ID_Levelness, Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)));
         // temporarily disabled because we are still on the fence with the hands on the back thing
         // animator.SetBool(ID_IsMovingFastOverTime, fastTravelTimer.TimeRemaining < 0f && !_isHoldingBall);
         
         animator.SetFloat(ID_SwimSpeed, (Mathf.Clamp01(_velocityTracker.SmoothSpeed) + _dashingGauge.FillAmount) * velocityDot);

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
         
         animator.SetFloat(ID_BallAnglePitch, _ballTracker.Angle1 / 90f);

     }

     void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position); // Always update first to validate the values
    }

    private void LateUpdate()
    {
        UpdateSimulatedLimbs();
    }

    void UpdateSimulatedLimbs()
    {
        
        float speedMultiplier = MathfUtils.RemapClamped(_velocityTracker.SmoothSpeed, 1.2f, 3.5f, 1, .4f);
        float directionDot = Vector3.Dot(transform.forward, _velocityTracker.SmoothVelocity.normalized);
        float directionMultiplier = MathfUtils.RemapClamped(directionDot, -.4f, 0, 0, 1);
        float multiplier = speedMultiplier * directionMultiplier;

        // Update Torso Transforms
        hipSpineChain.GetOriginal();
        var root = hipSpineChain.RootInverse;
        root.Rotate(Vector3.right, _angleTracker.Angle1 * 1f * multiplier);
        root.Rotate(Vector3.forward, -_angleTracker.Angle2 * multiplier);
        hipSpineChain.Apply();
        foreach (var t in lowerLegs)
        {
            t.Rotate(Vector3.right, -_angleTracker.Angle1 * 1f * multiplier);
        }
        
        // Update Twist Transforms
        
        float verticalAngle = _angleTracker.Angle1 * multiplier;
        float horizontalAngle = _angleTracker.Angle2 * multiplier;
        upperSpine.Rotate(Vector3.up, horizontalAngle * 0.5f);
        upperSpine.Rotate(Vector3.right, -verticalAngle * 0.5f);
        foreach (var t in arms)
        {
            var vectorYaw = t.InverseTransformDirection(transform.up);
            var vectorPitch = t.InverseTransformDirection(transform.right);
            t.Rotate(vectorYaw, horizontalAngle * 0.5f);
            t.Rotate(vectorPitch, verticalAngle * 0.7f);
        }
    }

    public void SetHoldingBall(bool newValue)
    {
        if (_isHoldingBall == newValue) return;
        _isHoldingBall = newValue;
        animator.SetInteger(ID_BallState, _isHoldingBall? 2 : 0);

        if (!_isHoldingBall)
        {
            fastTravelTimer.Reset();
        }

        if (_isHoldingBall)
        {
            animator.SetTrigger(ID_CancelAnimEffects);
        }
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
        legDelay.ResetTransform();
        _velocityTracker.ResetVelocities(transform.position);
        animator.SetTrigger(ID_CancelAnimEffects);
        animator.SetBool(ID_IsMovingFastOverTime, false);
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
        if(_ballTracker is not null) _ballTracker.DrawDebug();
    }
    #endif
}
