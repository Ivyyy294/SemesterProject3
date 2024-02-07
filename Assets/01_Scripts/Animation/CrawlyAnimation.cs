using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimUtils;
using UnityEngine;

public class CrawlyAnimation : MonoBehaviour
{
    [Header("External References")] 
    [SerializeField] private Ball ballLogic;
    
    [Header("Local References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DiverVerletBehavior verletBehavior;
    [SerializeField] private CrawlyVisuals crawlyVisuals;

    [Header("Parameters")] 
    [SerializeField] private float turnSpeedDegrees = 30f;
    [SerializeField] private float wiggleStrength = 2f;

    [Header("Joints References")] 
    [SerializeField] private List<Transform> lowerBodyChain;

    #region AnimatorParameterIDs

    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");
    private readonly int ID_IsCurledUp = Animator.StringToHash("IsCurledUp");
    private readonly int ID_IsBeingHeld = Animator.StringToHash("IsBeingHeld");
    private readonly int ID_CurlUp = Animator.StringToHash("CurlUp");

    #endregion

    private bool _playerManagerFound = false;
    private PlayerNetworkInfo[] _playerNetworkInfos;
    
    private VelocityTracker _velocityTracker;
    private Vector3 _movementVector;
    
    private Gauge _wiggleFactorGauge;
    private bool _wiggleActive = true;
    
    private TimeCounter _hurtTimer = new(2f);
    
    private AngleTracker _inertiaTracker;
    private StateListener<Ball, int> _playerIDListener;

    private void OnEnable()
    {
        // Currently OnEnable is being called a lot / whenever the ball is thrown. This will be fixed later
        // KEEP IN MIND THE IMPLICATIONS FOR NOW THOUGH
        
        ResetSimulations();
        _wiggleFactorGauge = new Gauge(4, 4);
        _wiggleFactorGauge.SetFillAmount(1);
        _playerIDListener = new(ballLogic, x => x.CurrentPlayerId, -1, OnPlayerIDUpdate);

        var playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null) 
            _playerNetworkInfos = playerManager.PlayerNetworkInfos.ToArray();
        else Debug.LogWarning($"CrawlyAnimation: No PlayerManager found. Some functionality is restricted!");

        if (ballLogic != null)
        {
            ballLogic.onBallThrown.AddListener(OnThrown);
            ballLogic.onBallCollided.AddListener(OnCollision);
        }
    }

    private void ResetSimulations()
    {
        _velocityTracker = new VelocityTracker(transform.position, 15);
        _movementVector = transform.forward;
        verletBehavior.ResetSimulation();
    }

    public void OnRoundStart()
    {
        ResetSimulations();
        animator.SetBool(ID_IsCurledUp, true);
        animator.SetTrigger(ID_CurlUp);
        crawlyVisuals.spherize = 1;
    }

    private void OnDisable()
    {
        animator.SetFloat(ID_SwimSpeed, 0);
    }

    private void Update()
    {
        _inertiaTracker = new(
            transform.position, 
            verletBehavior.GetLastNode(), 
            -transform.forward, 
            transform.right,
            transform.up);
        
        _wiggleFactorGauge.Update(_wiggleActive);
        _hurtTimer.Update();
        _playerIDListener.Update();

        animator.SetFloat(ID_SwimSpeed, _velocityTracker.SmoothSpeed);

        var beingHeld = _playerIDListener.CurrentValue != -1;
        if (beingHeld)
        {
            transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            HandleRotation(); // auto rotate into the direction of travel and reduce "twist"
            HandleScale(); // auto scale up when far away
        }
    }

    private void HandleRotation()
    {
        float levelness = 1 - Mathf.Abs(Vector3.Dot(transform.forward, Vector3.up));
        float twist = Mathf.Abs(Vector3.Dot(transform.right, Vector3.up));

        if (levelness > 0.2f && twist > 0.1f)
        {
            FromToRotateTowards(transform.right, GetIdealRightVector(), turnSpeedDegrees * 0.2f);
        }

        if (_velocityTracker.SmoothSpeed > 0.2f)
        {
            _movementVector = _velocityTracker.SmoothVelocity;
        }

        if (_movementVector != Vector3.zero && _velocityTracker.SmoothSpeed > 0.1f)
        {
            FromToRotateTowards(transform.forward, _movementVector, turnSpeedDegrees);
        }
    }
    private void HandleScale()
    {
        // test with multiple players
        
        if (_playerManagerFound)
        {
            PlayerNetworkInfo nearestPlayer = _playerNetworkInfos[0];
            float playerMinDistanceSqr = (nearestPlayer.transform.position - transform.position).sqrMagnitude;
            for (int i = 1; i < _playerNetworkInfos.Length; i++)
            {
                var info = _playerNetworkInfos[i];
                var currentDistanceSqr = (info.transform.position - transform.position).sqrMagnitude;
                if (currentDistanceSqr < playerMinDistanceSqr)
                {
                    playerMinDistanceSqr = currentDistanceSqr;
                }
            }
            var nearestDistance = Mathf.Sqrt(playerMinDistanceSqr);
            var newScale = MathfUtils.RemapClamped(nearestDistance, 4, 13, 1, 1.7f);
            transform.localScale = Vector3.one * newScale;
        }
    }

    private void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position);
    }

    private void LateUpdate()
    {
        if(_inertiaTracker != null) UpdateSimulatedLimbs();
    }

    private void UpdateSimulatedLimbs()
    {
        var pitchAngle = _inertiaTracker.Angle1;
        var yawAngle = _inertiaTracker.Angle2;

        var multiplier = wiggleStrength * _wiggleFactorGauge.FillAmount;
        multiplier /= lowerBodyChain.Count;
        
        for (int i = 0; i < lowerBodyChain.Count; i++)
        {
            var t = lowerBodyChain[i];
            t.Rotate(Vector3.right, pitchAngle * multiplier);
            t.Rotate(Vector3.forward, yawAngle * multiplier);
        }
    }

    Vector3 GetIdealRightVector()
    {
        return Vector3.Cross(Vector3.up, transform.forward).normalized;
    }

    private void FromToRotateTowards(Vector3 from, Vector3 to, float speed)
    {
        var relativeRotation = Quaternion.FromToRotation(from, to);
        var targetRotation = relativeRotation * transform.rotation;
        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * speed);
    }

    private void OnThrown(Vector3 force)
    {
        // transform.forward = force.normalized;
        verletBehavior.ResetSimulation();
        _wiggleActive = true;
        _wiggleFactorGauge.SetFillAmount(1);
    }

    private void OnCollision()
    {
      
        animator.SetBool(ID_IsCurledUp, true);
        _hurtTimer.Reset(()=>
        {
            animator.SetBool(ID_IsCurledUp, false);
            crawlyVisuals.spherize = 0; // this is redundancy for when the AnimatorBehavior should fail
        });
    }

    private void OnPlayerIDUpdate(int newID)
    {
        var beingHeld = newID != -1;
        _wiggleActive = !beingHeld;
        animator.SetBool(ID_IsBeingHeld, beingHeld);

        if (beingHeld)
        {
            animator.SetBool(ID_IsCurledUp, false);
        }
    }
}
