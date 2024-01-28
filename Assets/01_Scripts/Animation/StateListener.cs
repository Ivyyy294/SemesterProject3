using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StateListener<Target, Result> where Result : IEquatable<Result>
{
    private readonly Target _target;
    private readonly Func<Target, Result> _selector;
    private readonly UnityEvent<Result> _onValueChange;
    
    private Result _resultCache;

    public Result CurrentValue => _resultCache;

    public StateListener(Target target, Func<Target, Result> selector, Result initialValue, params UnityAction<Result>[] actions)
    {
        _target = target;
        _selector = selector;
        _resultCache = initialValue;
        _onValueChange = new UnityEvent<Result>();
        foreach (var action in actions)
        {
            Subscribe(action);
        }
    }

    ~StateListener()
    {
        _onValueChange.RemoveAllListeners();        
    }

    public void Update()
    {
        Result newValue = _selector(_target);
        if (EqualityComparer<Result>.Default.Equals(newValue, _resultCache)) return;
        _resultCache = newValue;
        _onValueChange.Invoke(newValue);
    }

    public void Subscribe(UnityAction<Result> action)
    {
        _onValueChange.AddListener(action);
    }

    public void Unsubscribe(UnityAction<Result> action)
    {
        _onValueChange.RemoveListener(action);
    }
}
