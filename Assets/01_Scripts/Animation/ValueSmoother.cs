using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSmoother<T>
{
    private T _targetA;
    private T _targetB;
    private T _smoothTarget;
    public T SmoothTarget => _smoothTarget;
    private float _timer;
    private Func<T, T, float, T> _lerpingFunction; 

    public ValueSmoother(T value, Func<T, T, float, T> lerpingFunction)
    {
        _smoothTarget = _targetB = _targetA = value;
        _lerpingFunction = lerpingFunction;
    }
    public void FixedUpdate(T newValue)
    {
        _targetA = _targetB;
        _targetB = newValue;
        _timer = 0f;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        var t = _timer / Time.fixedDeltaTime;
        _smoothTarget = _lerpingFunction(_targetA, _targetB, t);
    }

    public static ValueSmoother<float> FloatSmoother(float initialValue)
    {
        return new ValueSmoother<float>(initialValue, (a, b, t) => Mathf.Lerp(a, b, t));
    }

    public static ValueSmoother<Vector3> Vector3Smoother(Vector3 initialValue)
    {
        return new ValueSmoother<Vector3>(initialValue, (a, b, t) => Vector3.Lerp(a, b, t));
    }
}
