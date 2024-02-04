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
    [SerializeField] private PlayerThrowBall playerThrowBall;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerOxygen playerOxygen;

    [Header("Joints References")]
    [SerializeField] private MultiInverseChain spineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private Transform chest;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;
    [SerializeField] private List<Transform> neck;

    #region AnimatorParameterIDs
    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_Levelness = Animator.StringToHash("Levelness");
    private readonly int ID_BreastStrokeUB = Animator.StringToHash("BreastStrokeUB");
    private readonly int ID_CancelAnimEffects = Animator.StringToHash("CancelAnimEffects");
    private readonly int ID_IsBlocking = Animator.StringToHash("IsBlocking");
    
    private readonly int ID_BallFront = Animator.StringToHash("BallFront");
    private readonly int ID_BallUp = Animator.StringToHash("BallUp");
    private readonly int ID_BallDown = Animator.StringToHash("BallDown");
    private readonly int ID_BallSide = Animator.StringToHash("BallSide");
    private readonly int ID_IsBallHolding = Animator.StringToHash("IsBallHolding");
    private readonly int ID_IsReaching = Animator.StringToHash("IsReaching");
    private readonly int ID_IsBallLeft = Animator.StringToHash("IsBallLeft");
    private readonly int ID_Throw = Animator.StringToHash("Throw");
    #endregion
    
     private VelocityTracker _velocityTracker;
     
     [Header("Animation Timing")]

     [SerializeField] private Cooldown breastStrokeCooldown;
     [SerializeField] private float lowSpeedThreshold;

     private AnimUtils.AngleTracker _inertiaTracker;
     
     private StateListener<PlayerBallStatus, bool> _hasBallListener;
     private bool _isHoldingBall;
     private StateListener<PlayerBlock, bool> _isBlockingListener;
     private StateListener<PlayerOxygen, bool> _hasOxygenListener;
     private bool _oxygenEmpty;
     private Gauge _dashingGauge;

     private Cooldown _throwCooldown = new(0.5f);
     
     private void OnEnable()
     {
        verletBehavior.ResetSimulation();
        
        _isHoldingBall = false;
        _hasBallListener = new(playerBallStatus, x => x.HasBall(), _isHoldingBall, SetHoldingBall);
        _isBlockingListener = new(playerBlock, x => x.IsBlocking, false, SetBlocking);
        _hasOxygenListener = new(playerOxygen, x => x.OxygenEmpty, false, OnOxygenChanged);
        _velocityTracker = new VelocityTracker(transform.position, 24);
        
        breastStrokeCooldown.MakeReady();
        _dashingGauge = new Gauge(fillRate: 2, depletionRate: 3);
        
        animator.SetFloat(ID_SwimSpeed, 0);
        animator.SetBool(ID_IsBallHolding, false);
        animator.SetBool(ID_IsReaching, false);

        playerThrowBall.onBallThrow.AddListener(OnThrowBall);
    }

     void Update()
     {
         _hasBallListener.Update();
         _isBlockingListener.Update();
         _hasOxygenListener.Update();
         _dashingGauge.Update(playerMovement.IsDashing && _velocityTracker.SmoothSpeed > 2f);
         _throwCooldown.Update();

         _inertiaTracker = new(
             transform.position, 
             verletBehavior.GetLastNode(), 
             -transform.forward, 
             transform.right,
             transform.up);

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
         
         if (!_isHoldingBall && !_oxygenEmpty && _throwCooldown.IsReady && BallInRange())
         {
             animator.SetBool(ID_IsReaching, true);
             animator.SetTrigger(ID_CancelAnimEffects);
         }
         else animator.SetBool(ID_IsReaching, false);

         if (!_isHoldingBall)
         {
             var ballVector = (GetBallGrabPosition(chest.position) - chest.position).normalized;
             var sideDot = Vector3.Dot(ballVector, -transform.right);
             animator.SetBool(ID_IsBallLeft, sideDot > 0);
             animator.SetFloat(ID_BallFront, Mathf.Clamp01(Vector3.Dot(ballVector, transform.forward)));
             animator.SetFloat(ID_BallUp, Mathf.Clamp01(Vector3.Dot(ballVector, transform.up)));
             animator.SetFloat(ID_BallSide, Mathf.Abs(sideDot));
             animator.SetFloat(ID_BallDown, Mathf.Clamp01(Vector3.Dot(ballVector, -transform.up)));
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

    public void UpdateSimulatedLimbs()
    {
        float multiplier = 1.5f;
        var verticalAngle = _inertiaTracker.Angle1 * multiplier;
        var horizontalAngle = _inertiaTracker.Angle2 * multiplier;

        // Update Lower Body Transforms

        if (Input.GetKey(KeyCode.T)) return;
        spineChain.GetOriginal();
        for (int i = 0; i < spineChain.ChainLength - 1; i++)
        {
            var t = spineChain.GetInverse(i);
            t.Rotate(Vector3.right, verticalAngle * 0.3f);
            t.Rotate(Vector3.forward, -horizontalAngle * 0.3f);
        }
        
        spineChain.Apply();
        

        foreach (var t in lowerLegs)
        {
            var vectorPitch = t.InverseTransformDirection(transform.right);
            t.Rotate(vectorPitch, verticalAngle * 0.7f);
        }
        
        // Update Upper Body Twist Transforms

        if (!_isHoldingBall)
        {
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
        
        // Update Neck and Head

        foreach (var t in neck)
        {
            t.Rotate(Vector3.forward, horizontalAngle * 0.2f);
            t.Rotate(Vector3.right, -verticalAngle * 0.2f);
        }
    }

    public void OnThrowBall()
    {
        _throwCooldown.Reset();
        animator.SetBool(ID_IsBallHolding, false);
        animator.SetTrigger(ID_Throw);
    }

    public void SetHoldingBall(bool newValue)
    {
        if (_isHoldingBall == newValue) return;
        _isHoldingBall = newValue;
        
        animator.SetBool(ID_IsBallHolding, newValue);
        animator.SetBool(ID_IsReaching, false);
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
        return Ball.Me.transform.position;
    }

    private Vector3 GetBallGrabPosition(Vector3 origin)
    {
        var ball = GetBallTargetPosition();
        var delta = ball - origin;
        return origin + delta.normalized * Mathf.Min(0.656f, delta.magnitude - 0.22f);
    }
    
    private bool BallInRange()
    {
        var ball = Ball.Me;
        if (ball.CurrentPlayerId != -1) return false;
        var ballPos = ball.transform.position;
        float armReach = 2;
        if ((ballPos - chest.position).magnitude < armReach) return true;
        var transitionTime = 0.2f;
        var projectedBallPosition = ballPos + ball.Velocity * transitionTime;
        var projectedPlayerPosition = chest.position + _velocityTracker.SmoothVelocity * transitionTime;
        if ((projectedBallPosition - projectedPlayerPosition).magnitude < armReach) return true;
        return false;
    }

    private void OnOxygenChanged(bool isEmpty)
    {
        if (_oxygenEmpty == isEmpty) return;
        _oxygenEmpty = isEmpty;
        if (isEmpty)
        {
            animator.SetBool(ID_IsBallHolding, false);
            animator.SetBool(ID_IsReaching, false);
            animator.SetTrigger(ID_CancelAnimEffects);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if(_angleTracker is not null) _angleTracker.DrawDebug();
        // spineChain.DrawGizmos(0.05f);
    }
    #endif
}
