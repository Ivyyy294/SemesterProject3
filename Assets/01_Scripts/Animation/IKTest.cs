using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private Transform testJoint;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        testJoint.position += new Vector3(0, Mathf.Sin(Time.time * 3), 0);
        Debug.Log(testJoint.position);
    }
    
}
