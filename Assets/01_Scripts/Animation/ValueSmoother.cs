using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSmoother
{
    private Vector3 _targetA;
    private Vector3 _targetB;
    private Vector3 _smoothTarget;
    public Vector3 SmoothTarget => _smoothTarget;
    private float _timer;

    public ValueSmoother(Vector3 value)
    {
        _smoothTarget = _targetB = _targetA = value;
    }
    public void FixedUpdate(Vector3 newValue)
    {
        _targetA = _targetB;
        _targetB = newValue;
        _timer = 0f;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        var t = _timer / Time.fixedDeltaTime;
        _smoothTarget = _targetA * (1 - t) + _targetB * t;
    }
}
