using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[Serializable]
public struct RigArm
{
    public Transform target;
    public Transform hint;
    public Rig ikRig;
    public Rig twistRig;
    public Transform root;
    public Transform elbow;
    public Transform hand;

    public List<Rig> Rigs => new() { ikRig, twistRig };
}

public class DiverIKController : MonoBehaviour
{
    [Header("Left Arm")]
    public RigArm leftArm;
    [Header("Right Arm")]
    public RigArm rightArm;
    [Header("Other Rigs")]
    public Rig chestTwistUp;
    public Rig chestTwistDown;
    public Rig leftHandRotation;

    public List<Rig> Rigs => leftArm.Rigs.Concat(rightArm.Rigs).Concat(new []{chestTwistUp, chestTwistDown, leftHandRotation}).ToList();
}
