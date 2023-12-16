using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DiverInput))]
public class DiverMovement : MonoBehaviour
{
    [SerializeField] private Transform diverModel;
    [SerializeField] private Transform diverModel2;
    [SerializeField] private Transform torso;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float turnSpeedDegrees = 10f;

    private DiverInput diverInput;

    private void Awake()
    {
        diverInput = GetComponent<DiverInput>();
    }

    void Update()
    {
        diverModel.Rotate(Vector3.right, Time.deltaTime * turnSpeedDegrees * diverInput.Pitch);
        diverModel.Rotate(Vector3.up, Time.deltaTime * turnSpeedDegrees * diverInput.Yaw);
        
        // how horizontal the diver is, is needed to ONLY auto-correct when the diver a little horizontal
        float levelness = 1 - Mathf.Abs(Vector3.Dot(diverModel.forward, Vector3.up));
        // how vertical the diver's local RIGHT axis is, is needed to allow loopings
        float twist = Mathf.Abs(Vector3.Dot(diverModel.right, Vector3.up));
        
        if (levelness > 0.2f && twist > 0.1f)
        {
            // gradually roll the diver until their local RIGHT axis is horizontal again (their hip-line is horizontal)
            var relativeRotation = Quaternion.FromToRotation(diverModel.right, GetIdealRightVector());
            var targetRotation = relativeRotation * diverModel.rotation;
            diverModel.rotation = Quaternion.RotateTowards(diverModel.rotation, targetRotation, Time.deltaTime * turnSpeedDegrees * 0.5f);
        }

        float adjustedMovementSpeed = GetForwardMotionMultiplier() * movementSpeed;
        transform.position += adjustedMovementSpeed * Time.deltaTime * diverModel.forward;
        
        // create delayed motion in the body parts
        UpdateTwistTransforms();
        UpdateTorsoTransforms();
    }

    void UpdateTwistTransforms()
    {
        diverModel2.localEulerAngles = new Vector3(0, 0, -diverInput.YawSway * 30f);
    }

    void UpdateTorsoTransforms()
    {
        torso.localEulerAngles = new Vector3(-diverInput.PitchSway * 70f, -diverInput.YawSway * 40f, 0);
    }
    
    Vector3 GetIdealRightVector()
    {
        return Vector3.Cross(Vector3.up, diverModel.forward).normalized;
    }

    float GetForwardMotionMultiplier()
    {
        // cheap acceleration / deceleration approximation
        float result = diverInput.ForwardTime;
        if (result < 0)
        {
            result = 1 - (diverInput.ForwardTime * -1);
        }

        return Mathf.Clamp01(result);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {   
        // visualize the target orientation for the hips of the diver
        Gizmos.DrawLine(diverModel.position, diverModel.position + GetIdealRightVector());
    }
    #endif
}
