using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InverseChain))]
public class DiverAnimation : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] private TransformDelay legDelay;
    [SerializeField] private PlayerBallStatus playerBallStatus;
    [SerializeField] private PlayerBlock playerBlock;
    
    [Header("Joints References")]
    [SerializeField] private InverseChain hipSpineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;

    #region AnimatorParameterIDs
    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_Levelness = Animator.StringToHash("Levelness");
    private readonly int ID_IsHoldingBall = Animator.StringToHash("IsHoldingBall");
    private readonly int ID_IsMovingFastOverTime = Animator.StringToHash("IsMovingFastOverTime");
    private readonly int ID_BreastStroke = Animator.StringToHash("BreastStrokeUB");
    private readonly int ID_CancelAnimEffects = Animator.StringToHash("CancelAnimEffects");
    private readonly int ID_IsBlocking = Animator.StringToHash("IsBlocking");
    #endregion
    
     private VelocityTracker _velocityTracker;
     
     [Header("Animation Timing")]
     [SerializeField] private TimeCounter fastTravelTimer;
     [SerializeField] private float fastTravelSpeedThreshold;

     [SerializeField] private Cooldown breastStrokeCooldown;
     [SerializeField] private float lowSpeedThreshold;

     private AnimUtils.AngleTracker _angleTracker;
     private bool _isHoldingBall;

     private StateListener<PlayerBallStatus, bool> _hasBallListener;
     private StateListener<PlayerBlock, bool> _isBlockingListener;

     private void OnEnable()
    {
        animator.SetFloat(ID_SwimSpeed, 0);
        _velocityTracker = new VelocityTracker(transform.position, 24);
        _isHoldingBall = false;
        breastStrokeCooldown.MakeReady();
        _hasBallListener = new(playerBallStatus, x => x.HasBall(), false, SetHoldingBall);
        _isBlockingListener = new(playerBlock, x => x.IsBlocking, false, SetBlocking);
    }

     void Update()
     {
         _hasBallListener.Update();
         _isBlockingListener.Update();
         
         _angleTracker = new(transform.position, legDelay.DelayPosition, -transform.forward, transform.right, transform.up);

         if (_velocityTracker.SmoothSpeed > fastTravelSpeedThreshold)
         {
             fastTravelTimer.Update();
         } else fastTravelTimer.Reset();

         if (_velocityTracker.SmoothSpeed < lowSpeedThreshold)
         {
             breastStrokeCooldown.Update();
         }
         else if(!_isHoldingBall)
         {
             if(breastStrokeCooldown.Trigger()) animator.SetTrigger(ID_BreastStroke);
             breastStrokeCooldown.Reset();
         }

         animator.SetFloat(ID_Levelness, Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)));
         // animator.SetBool(ID_IsMovingFastOverTime, fastTravelTimer.TimeRemaining < 0f && !_isHoldingBall);
         
         animator.SetFloat(ID_SwimSpeed, Mathf.Max(0, _velocityTracker.SmoothSpeed));

         if (Input.GetKeyDown(KeyCode.E))
         {
             ToggleHoldingBall();
         }
         
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

        // Update Torso Transforms
        hipSpineChain.GetOriginal();
        var root = hipSpineChain.RootInverse;
        root.Rotate(Vector3.right, _angleTracker.Angle1 * 1f * speedMultiplier);
        root.Rotate(Vector3.forward, -_angleTracker.Angle2 * speedMultiplier);
        hipSpineChain.Apply();
        foreach (var t in lowerLegs)
        {
            t.Rotate(Vector3.right, -_angleTracker.Angle1 * 1f * speedMultiplier);
        }
        
        // Update Twist Transforms
        
        float verticalAngle = _angleTracker.Angle1 * speedMultiplier;
        float horizontalAngle = _angleTracker.Angle2 * speedMultiplier;
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
        animator.SetBool(ID_IsHoldingBall, _isHoldingBall);

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_angleTracker is not null) _angleTracker.DrawDebug();
    }
    #endif
}
