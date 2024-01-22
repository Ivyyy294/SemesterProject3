using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlyAnimation : MonoBehaviour
{
    [Header("Local References")]
    [SerializeField] private Animator animator;

    [Header("Parameters")] [SerializeField]
    private float turnSpeedDegrees = 30f;

    #region AnimatorParameterIDs

    private readonly int ID_SwimSpeed = Animator.StringToHash("SwimSpeed");

    #endregion

    private VelocityTracker _velocityTracker;
    private Vector3 _movementVector;

    private void OnEnable()
    {
        _velocityTracker = new VelocityTracker(transform.position, 8);
        _movementVector = transform.forward;
    }

    private void Update()
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

        animator.SetFloat(ID_SwimSpeed, _velocityTracker.SmoothSpeed);
    }

    private void FixedUpdate()
    {
        _velocityTracker.FixedUpdate(transform.position);
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
