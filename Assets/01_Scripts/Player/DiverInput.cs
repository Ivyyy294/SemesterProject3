using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ivyyy.Network;

public class DiverInput : NetworkBehaviour
{
    private float _forwardTime = Mathf.NegativeInfinity;
    private float _pitch;
    private float _pitchSway = 0;
    private float _yaw;
    private float _yawSway = 0;

    public UnityEvent startForward;
    public UnityEvent stopForward;

    public float ForwardTime => _forwardTime;
    public bool IsGoingForward => _forwardTime > 0f;
    public float Pitch => _pitch;
    public float PitchSway => _pitchSway;
    public float Yaw => _yaw;
    public float YawSway => _yawSway;
    
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (_forwardTime));
		networkPackage.AddValue (new NetworkPackageValue (_pitch));
		networkPackage.AddValue (new NetworkPackageValue (_pitchSway));
		networkPackage.AddValue (new NetworkPackageValue (_yaw));
		networkPackage.AddValue (new NetworkPackageValue (_yawSway));
	}

    void Update()
    {
		if (Owner)
		{
			UpdateForwardInput();
			UpdatePitch();
			UpdateYaw();
		}
		else
		{
			if (networkPackage.Count == 5)
			{
				_forwardTime = networkPackage.Value(0).GetFloat();
				_pitch = networkPackage.Value(1).GetFloat();
				_pitchSway = networkPackage.Value(2).GetFloat();
				_yaw = networkPackage.Value(3).GetFloat();
				_yawSway = networkPackage.Value(4).GetFloat();
			}
		}
    }

    void UpdateForwardInput()
    {
        var forwardKey = KeyCode.Space;
        if (Input.GetKeyDown(forwardKey))
        {
            _forwardTime = Time.deltaTime;
            startForward.Invoke();
        }

        if (Input.GetKeyUp(forwardKey))
        {
            _forwardTime = -Time.deltaTime;
            stopForward.Invoke();
        }
        if (Input.GetKey(forwardKey))
        {
            _forwardTime += Time.deltaTime;
        }
        else
        {
            _forwardTime -= Time.deltaTime;
        }
    }
    void UpdatePitch()
    {
        _pitch = Input.GetAxisRaw("Vertical");
        float diff = Mathf.Sign(_pitch - _pitchSway);
        _pitchSway = Mathf.Clamp(_pitchSway + diff * Time.deltaTime, -1, 1);
    }

    void UpdateYaw()
    {
        _yaw = Input.GetAxisRaw("Horizontal");
        float diff = Mathf.Sign(_yaw - _yawSway);
        _yawSway = Mathf.Clamp(_yawSway + diff * Time.deltaTime, -1, 1);
    }
}
