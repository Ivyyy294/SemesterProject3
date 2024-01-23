using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimUtils;
using UnityEngine;

public class CrawlyAnimation : MonoBehaviour
{
    [Header("Local References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DiverVerletBehavior verletBehavior;

    [Header("Parameters")] 
    [SerializeField] private float turnSpeedDegrees = 30f;
    [SerializeField] private float wiggleStrength = 2f;

    [Header("Joints References")] 
    [SerializeField] private List<Transform> lowerBodyChain;

    #region AnimatorParameterIDs

    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");

    #endregion

    private bool _playerManagerFound = false;
    private PlayerNetworkInfo[] _playerNetworkInfos;
    
    private VelocityTracker _velocityTracker;
    private Vector3 _movementVector;
    
    private Gauge _wiggleFactorGauge;
    private bool _wiggleActive = true;
    
    private AngleTracker _inertiaTracker;

    private void OnEnable()
    {
        _velocityTracker = new VelocityTracker(transform.position, 8);
        _movementVector = transform.forward;
        verletBehavior.ResetSimulation();
        _wiggleFactorGauge = new Gauge(4, 4);
        _wiggleFactorGauge.SetFillAmount(1);

        var playerManager = FindObjectOfType<PlayerManager>();
        _playerManagerFound = playerManager != null;
        if (_playerManagerFound) 
            _playerNetworkInfos = FindObjectOfType<PlayerManager>().PlayerNetworkInfos.ToArray();
        else Debug.LogWarning($"CrawlyAnimation: No PlayerManager found. Some functionality is restricted!");
    }

    private void Update()
    {
        _inertiaTracker = new(
            transform.position, 
            verletBehavior.SmoothTarget, 
            -transform.forward, 
            transform.right,
            transform.up);
        
        _wiggleFactorGauge.Update(_wiggleActive);

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

        animator.SetFloat(ID_SwimSpeed, _velocityTracker.SmoothSpeed);
        
        // test with multiple players
        
        if (_playerManagerFound)
        {
            PlayerNetworkInfo nearestPlayer = _playerNetworkInfos[0];
            float playerMinDistanceSqr = (nearestPlayer.transform.position - transform.position).sqrMagnitude;
            int nearestPlayerIndex = 0;
            for (int i = 1; i < _playerNetworkInfos.Length; i++)
            {
                var info = _playerNetworkInfos[i];
                var currentDistanceSqr = (info.transform.position - transform.position).sqrMagnitude;
                if (currentDistanceSqr < playerMinDistanceSqr)
                {
                    nearestPlayer = info;
                    playerMinDistanceSqr = currentDistanceSqr;
                    nearestPlayerIndex = i;
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
}
