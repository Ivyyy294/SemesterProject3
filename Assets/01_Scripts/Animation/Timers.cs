using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public float cooldownTime;
    public bool IsReady => _cooldown <= 0;
    public float TimeRemaining => _cooldown;
    public float ReadySince => -_cooldown;
    
    private float _cooldown;

    private Action _onReadyAction;

    public Cooldown(float cooldownTime = 1)
    {
        this.cooldownTime = cooldownTime;
    }

    public void Update()
    {
        _cooldown -= Time.deltaTime;
        if (IsReady && !(_onReadyAction is null))
        {
            _onReadyAction();
            _onReadyAction = null;
        }
    }

    public void Reset()
    {
        _cooldown = cooldownTime;
    }

    public bool Trigger()
    {
        if (!IsReady) return false;
        
        Reset();
        return true;
    }

    public bool Trigger(Action onReady)
    {
        var result = Trigger();
        if(result) _onReadyAction = onReady;
        return result;
    }

    public void MakeReady()
    {
        _cooldown = 0f;
    }
}

[Serializable]
public class Timer
{
    public float waitTime;

    public float TimeRemaining => waitTime - _timer;
    public float ProgressNormalized => waitTime == 0? 1 : _timer / waitTime;
    public bool IsRunning => _isActive && TimeRemaining > 0f; 

    private float _timer;
    private bool _isActive;
    private Action _onTimeOverAction;

    public Timer(float waitTime = 1f)
    {
        this.waitTime = waitTime;
    }

    public void Update()
    {
        if (_isActive) _timer += Time.deltaTime;
        if (TimeRemaining <= 0 && _onTimeOverAction != null)
        {
            _onTimeOverAction();
            _onTimeOverAction = null;
        }
    }

    public void Start()
    {
        _timer = 0f;
        Continue();
    }

    public void Start(Action onTimeOverAction)
    {
        _onTimeOverAction = onTimeOverAction;
        Start();
    }

    public void Stop()
    {
        _isActive = false;
    }

    public void Continue()
    {
        _isActive = true;
    }

    public void Reset()
    {
        Stop();
        _timer = 0;
    }

    public bool Trigger()
    {
        if (!_isActive) return false;
        if (_timer < waitTime) return false;
        Reset();
        return true;
    }
}

[Serializable]
public class TimeCounter
{
    public float waitTime;
    private float _timer;
    private Action _onTimeOverAction;

    public float Timer => _timer;
    public float TimeRemaining => waitTime - _timer;
    public float ProgressNormalized => waitTime == 0? 1 : _timer / waitTime;

    public TimeCounter(float waitTime)
    {
        this.waitTime = waitTime;
    }
    
    public void Update()
    {
        _timer += Time.deltaTime;
        if (TimeRemaining <= 0 && _onTimeOverAction != null)
        {
            _onTimeOverAction();
            _onTimeOverAction = null;
        }
    }

    public void Reset()
    {
        _timer = 0;
    }

    public void Reset(Action onTimeOverAction)
    {
        _onTimeOverAction = onTimeOverAction;
        Reset();
    }

    public bool Trigger()
    {
        if (_timer <= waitTime) return false;
        Reset();
        return true;
    }

    public void MakeReady()
    {
        _timer = waitTime;
    }
}

[Serializable]
public class Gauge
{
    public float fillRate;
    public float depletionRate;
    
    public float FillAmount => _fillAmount;
    public float OverflowAmount => _overflowGauge;
    public bool IsFull => _fillAmount >= 1 && !IsOverflowing;
    public bool IsEmpty => _fillAmount <= 0;

    public bool IsOverflowing => _overflowGauge > 0;
    
    private float _fillAmount;
    private float _overflowGauge;

    public Gauge(float fillRate, float depletionRate)
    {
        this.fillRate = fillRate;
        this.depletionRate = depletionRate;
    }

    public void Fill()
    {
        _fillAmount += fillRate * Time.deltaTime;
        ClampFill();
    }

    public void Deplete()
    {
        _overflowGauge -= depletionRate * Time.deltaTime;
        if (_overflowGauge < 0)
        {
            _fillAmount += _overflowGauge;
            _overflowGauge = 0;
        }
        ClampFill();
    }

    public void Update(bool state)
    {
        if(state) Fill();
        else Deplete();
    }

    public void SetFillAmount(float amount, bool overflow = false)
    {
        _fillAmount = amount;
        if (overflow && _fillAmount > 1)
        {
            _overflowGauge = _fillAmount - 1f;
        }
        else _overflowGauge = 0;
        ClampFill();
    }

    public void ChangeFillAmount(float difference, bool overflow = false)
    {
        SetFillAmount(_fillAmount + difference, overflow: overflow);
    }

    public void Reset()
    {
        _fillAmount = 0;
        _overflowGauge = 0;
    }

    private void ClampFill()
    {
        _fillAmount = Mathf.Clamp01(_fillAmount);
    }
}
