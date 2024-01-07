using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum AnimationStyle
{
    Bounce,
    Interrupt,
    Await
}

public enum AnyAnimationState
{
    Idle,
    Forward,
    Backwards,
}

public class AnimationBouncer : MonoBehaviour
{
    public string identifier;
    public List<AnimationCurve> curves;
    public AnimationStyle animationStyle = AnimationStyle.Bounce;
    public float animationTime = 1f;

    public float Progress => _progress;

    public AnyAnimationState AnimationState => IsAnimationCapped() ? 
            AnyAnimationState.Idle : _direction > 0.5 ? 
                AnyAnimationState.Forward : AnyAnimationState.Backwards;

    private float _progress = 0f;
    private int _direction = -1;
    private int _reverseCues = 0;
    private bool _isCappedCache = true;
    private List<UnityAction> _onCompleteActions;

    private void Start()
    {
        _onCompleteActions = new List<UnityAction>();
    }

    private void Update()
    {
        _progress += (Time.deltaTime / animationTime) * _direction;
        _progress = Mathf.Clamp01(_progress);
        if (IsAnimationCapped())
        {
            if (!_isCappedCache)
            {
                OnAnimationCapped();
            }

            _isCappedCache = true;
            if (_reverseCues > 0)
            {
                _isCappedCache = true;
                _reverseCues--;
                _direction *= -1;
            }
        }
        else
        {
            _isCappedCache = false;
        }
        
    }
    
    private bool IsAnimationCapped()
    {
        return _progress is <= 0f or >= 1f;
    }
    
    public void PlayForward(UnityAction onComplete = null)
    {
        if (animationStyle == AnimationStyle.Await)
        {
            IncrementReverseCues();
            AddCompleteAction(onComplete);
            return;
        }
        if (_direction == 1)
        {
            return;
        }
        _direction = 1;
        AddCompleteAction(onComplete);
        if (animationStyle == AnimationStyle.Interrupt) _progress = 0;
    }

    public void PlayBackwards(UnityAction onComplete = null)
    {
        if (animationStyle == AnimationStyle.Await && _direction == 1)
        {
            IncrementReverseCues();
            AddCompleteAction(onComplete);
            return;
        }
        if (_direction == -1)
        {
            return;
        }
        _direction = -1;
        AddCompleteAction(onComplete);
        if (animationStyle == AnimationStyle.Interrupt) _progress = 1;
    }

    public float Evaluate(int index)
    {
        return curves[index].Evaluate(_progress);
    }

    private void IncrementReverseCues()
    {
        _reverseCues++;
    }

    private void OnAnimationCapped()
    {
        if (_onCompleteActions.Count > 0)
        {
            _onCompleteActions[0]();
            _onCompleteActions.RemoveAt(0);
        }
    }

    private void AddCompleteAction(UnityAction action)
    {
        if (action == null)
        {
            return;
        }
        if (animationStyle != AnimationStyle.Await)
        {
            _onCompleteActions.Clear();
        }
        _onCompleteActions.Add(action);
    }

    public static AnimationBouncer GetByIdentifier(GameObject go, string identifier)
    {
        var components = go.GetComponents<AnimationBouncer>();
        AnimationBouncer result = null;
        foreach (var c in components)
        {
            if (c.identifier == identifier) result = c;
        }
        return result;
    }
}
