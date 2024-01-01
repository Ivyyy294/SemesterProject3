using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ivyyy.Network;

public class DiverInput : NetworkBehaviour
{
    private float _pitch;
    private float _pitchSway = 0;
    private float _yaw;
    private float _yawSway = 0;
	private bool forwardPressed = false;

    public float Pitch => _pitch;
    public float PitchSway => _pitchSway;
    public float Yaw => _yaw;
    public float YawSway => _yawSway;
	public bool ForwardPressed => forwardPressed;

	[Header ("Key Bindings")]
	KeyCode forwardKey = KeyCode.Space;
    
	Rigidbody _rigidbody;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (_pitch));				//0
		networkPackage.AddValue (new NetworkPackageValue (_pitchSway));			//1
		networkPackage.AddValue (new NetworkPackageValue (_yaw));				//2
		networkPackage.AddValue (new NetworkPackageValue (_yawSway));			//3
		networkPackage.AddValue (new NetworkPackageValue (transform.position));	//4
		networkPackage.AddValue (new NetworkPackageValue (transform.rotation));	//5
		networkPackage.AddValue (new NetworkPackageValue (forwardPressed));		//6
	}

	void Start()
	{
		if (NetworkManager.Me is null) Owner = true;
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
    {
		UpdateForwardInput();
		UpdatePitch();
		UpdateYaw();

		if (!Owner)
			UpdatePositionData();
    }

    void UpdateForwardInput()
    {
		if (Owner)
			forwardPressed = Input.GetKey(forwardKey);
		else if (networkPackage.Available)
			forwardPressed = networkPackage.Value(6).GetBool();
    }

    void UpdatePitch()
    {
		if (Owner)
		{
			_pitch = Input.GetAxisRaw("Vertical");
			float diff = Mathf.Sign(_pitch - _pitchSway);
			_pitchSway = Mathf.Clamp(_pitchSway + diff * Time.deltaTime, -1, 1);
		}
		else if (networkPackage.Available)
		{
			_pitch = networkPackage.Value(1).GetFloat();
			_pitchSway = networkPackage.Value(2).GetFloat();
		}
    }

    void UpdateYaw()
    {
		if (Owner)
		{
			_yaw = Input.GetAxisRaw("Horizontal");
			float diff = Mathf.Sign(_yaw - _yawSway);
			_yawSway = Mathf.Clamp(_yawSway + diff * Time.deltaTime, -1, 1);
		}
		else if (networkPackage.Available)
		{
			_yaw = networkPackage.Value(2).GetFloat();
			_yawSway = networkPackage.Value(3).GetFloat();
		}
    }

	void UpdatePositionData()
	{
		if (networkPackage.Available)
		{
			transform.position = networkPackage.Value(4).GetVector3();
			transform.rotation = networkPackage.Value(5).GetQuaternion();
		}
	}
}
