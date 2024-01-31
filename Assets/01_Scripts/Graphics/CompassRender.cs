using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompassRender : MonoBehaviour
{
    [SerializeField] private Transform cover;
    [SerializeField] private Transform arrow;

    [SerializeField] private AnimationCurve coverAnimation;
    [SerializeField] private bool coverOpen;

    private Ball _ball;
    private Transform _pov;
    private bool _setupEnabled = false;
    private Gauge _coverOpenGauge = new Gauge(1, 1);
    private int _localPlayerIndex;


    private void Start()
    {
        Enable();
    }

    public void Enable()
    {
        var ball = Ball.Me;
        var cam = CameraSystem.Me;

        if (cam == null || ball ==  null)
        {
            _setupEnabled = false;
            return;
        }

        _localPlayerIndex = PlayerConfigurationManager.LocalPlayerId;
        _ball = ball;
        _pov = cam.MainCamera.transform;
        _coverOpenGauge.SetFillAmount(0);
        _setupEnabled = true;
    }

    private void Update()
    {

        coverOpen = (_ball.CurrentPlayerId != _localPlayerIndex);
        
        _coverOpenGauge.Update(coverOpen);

        var animSample = coverAnimation.Evaluate(_coverOpenGauge.FillAmount);
        if (!coverOpen)
        {
            animSample = 1 - coverAnimation.Evaluate(1 - _coverOpenGauge.FillAmount);
        }
        var angle = MathfUtils.Remap(animSample, 0, 1, -90, 90);
        cover.localEulerAngles = new Vector3(angle, 0, 0);
        
        if (!_setupEnabled) return;
        var relativePosition = _pov.InverseTransformPoint(_ball.transform.position);
        arrow.rotation = Quaternion.LookRotation(relativePosition);
    }
}
