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
    [SerializeField] private DiverIKController ikController;

    [Header("External Dependencies")]
    [SerializeField] private PlayerBallStatus playerBallStatus;
    [SerializeField] private PlayerThrowBall playerThrowBall;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Joints References")]
    [SerializeField] private MultiInverseChain spineChain;
    [SerializeField] private Transform upperSpine;
    [SerializeField] private Transform chest;
    [SerializeField] private List<Transform> lowerLegs;
    [SerializeField] private List<Transform> arms;

    #region AnimatorParameterIDs
    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_Levelness = Animator.StringToHash("Levelness");
    private readonly int ID_BreastStrokeUB = Animator.StringToHash("BreastStrokeUB");
    private readonly int ID_CancelAnimEffects = Animator.StringToHash("CancelAnimEffects");
    private readonly int ID_IsBlocking = Animator.StringToHash("IsBlocking");
    private readonly int ID_BallState = Animator.StringToHash("BallState");
    #endregion
    
     private VelocityTracker _velocityTracker;
     
     [Header("Animation Timing")]

     [SerializeField] private Cooldown breastStrokeCooldown;
     [SerializeField] private float lowSpeedThreshold;

     private AnimUtils.AngleTracker _inertiaTracker;
     private bool _isHoldingBall;

     private StateListener<PlayerBallStatus, bool> _hasBallListener;
     private StateListener<PlayerBlock, bool> _isBlockingListener;
     private Gauge _dashingGauge;

     private Gauge _ikStrengthGauge = new (2, 2);
     private Cooldown _throwCooldown = new(0.5f);
     private Gauge _ikArmSwitcher = new(2, 2);
     

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
        
        playerThrowBall.onBallThrow.AddListener(OnThrowBall);
    }

     void Update()
     {
         _hasBallListener.Update();
         _isBlockingListener.Update();
         _dashingGauge.Update(playerMovement.IsDashing && _velocityTracker.SmoothSpeed > 2f);
         _ikStrengthGauge.Update(BallInRange() && _throwCooldown.IsReady);
         _throwCooldown.Update();

         _inertiaTracker = new(
             transform.position, 
             verletBehavior.SmoothTarget, 
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

         HandleIK(false);

     }

     void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position); // Always update first to validate the values
    }

    private void LateUpdate()
    {
        UpdateSimulatedLimbs();
        
        // if (_ikEnabled)
        // {
        //     ikController.Apply();
        //     ikController.leftArm.lowerArm.original.Rotate(0, 90, 0);
        //     ikController.leftArm.hand.original.localEulerAngles = new Vector3(-11f, -6.4f, -39.21f);
        // }
    }

    private void HandleIK(bool ikEnabled)
    {
        if (_ikStrengthGauge.IsEmpty || !ikEnabled)
        {
            foreach (var r in ikController.Rigs)
            {
                r.weight = 0;
            }

            return;
        }

        var ikStrength = _ikStrengthGauge.FillAmount;

        var ballPos = GetBallTargetPosition();

        var twistRollAngle = AnimUtils.AngleTracker.GetAngleOnProjectedPlane(transform.position, ballPos, transform.forward,
            -transform.up);
        var twistPitchAngle = AnimUtils.AngleTracker.GetAngleOnProjectedPlane(transform.position, ballPos,
            transform.right, transform.forward);
        ikController.leftArm.twistRig.weight = -(twistRollAngle / 360) * ikStrength;
        ikController.rightArm.twistRig.weight = (twistRollAngle / 360) * ikStrength;
        ikController.chestTwistDown.weight = (twistPitchAngle / 360) * ikStrength;
        ikController.chestTwistUp.weight = -(twistPitchAngle / 360) * ikStrength;

        var leftSide = twistRollAngle < 0;
        _ikArmSwitcher.Update(leftSide);
        var armBias = _ikArmSwitcher.FillAmount * 2 - 1;

        SolveArmIK(ikController.leftArm, ikStrength * Mathf.Clamp01(armBias), transform.right);
        SolveArmIK(ikController.rightArm, ikStrength * Mathf.Clamp01(-armBias), -transform.right);
    }

    public void SolveArmIK(RigArm arm, float weight, Vector3 sideVector)
    {
        arm.ikRig.weight = weight;
        var ballPos = GetBallTargetPosition();
        var chestPos = chest.position;
        var ballGrabPos = GetBallGrabPosition(chestPos);
        var ballVector = ballPos - chestPos;
        arm.target.position = ballGrabPos;

        // position the pole target
        var ballHeightDot = Vector3.Dot(ballVector.normalized, transform.up);
        var ballForwardDot = Vector3.Dot(ballVector.normalized, transform.forward);
        var ballRightDot = Vector3.Dot(ballVector, -sideVector);

        var shoulderEndPos = arm.root.position;
        var shoulderToGrab = (ballGrabPos - shoulderEndPos);
        
        var initialHintPos = Vector3.Lerp(shoulderEndPos, ballGrabPos, 0.5f);
        var hintPos = Vector3.Cross(shoulderToGrab, transform.up).normalized;
        hintPos += transform.up * MathfUtils.RemapClamped(ballHeightDot, 0.3f, -1, 0, 3);
        hintPos += transform.up * MathfUtils.RemapClamped(ballForwardDot, 0, 1, 0, 1);
        var outwards = MathfUtils.RemapClamped(ballRightDot, 0.1f, 0.4f, 1, 0);
        hintPos += sideVector * outwards;
        hintPos = Vector3.Lerp(hintPos, initialHintPos + Vector3.up,
            MathfUtils.RemapClamped(ballForwardDot, 0.1f, -0.4f, 0, 1));
        
        // Limit hint Pos
        if(Vector3.Dot(hintPos - shoulderEndPos, sideVector) > 0)
            hintPos = AnimUtils.AngleTracker.GetProjectedPoint(shoulderEndPos, hintPos, sideVector);
        arm.hint.position = hintPos;

        var localArmUp = Vector3.Cross(shoulderToGrab, (ballGrabPos - hintPos)).normalized;
        arm.target.rotation = Quaternion.LookRotation(localArmUp, ballVector);
    }

    public void UpdateSimulatedLimbs()
    {
        // float speedMultiplier = MathfUtils.RemapClamped(_velocityTracker.SmoothSpeed, 1.2f, 3.5f, 1, .5f);
        // float directionDot = Vector3.Dot(transform.forward, _velocityTracker.SmoothVelocity.normalized);
        // float directionMultiplier = MathfUtils.RemapClamped(directionDot, -.4f, 0, 0, 1);
        // float multiplier = speedMultiplier * directionMultiplier;
        float multiplier = 1.5f;

        // Update Lower Body Transforms

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
        
        // Update Upper Body Twist Transforms
        
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

    public void OnThrowBall()
    {
        Debug.Log("Throwing Ball");
        _throwCooldown.Reset();
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
        return (ball.transform.position - chest.position).magnitude < 3;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // if(_angleTracker is not null) _angleTracker.DrawDebug();
        // spineChain.DrawGizmos(0.05f);
    }
    #endif
}
