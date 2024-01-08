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
    
	[Header ("Lara Values")]
	[SerializeField] Transform targetTransform;

	int packageNr = 0;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (_pitch));						//0
		networkPackage.AddValue (new NetworkPackageValue (_pitchSway));					//1
		networkPackage.AddValue (new NetworkPackageValue (_yaw));						//2
		networkPackage.AddValue (new NetworkPackageValue (_yawSway));					//3
		networkPackage.AddValue (new NetworkPackageValue (targetTransform.position));	//4
		networkPackage.AddValue (new NetworkPackageValue (targetTransform.rotation));	//5
		networkPackage.AddValue (new NetworkPackageValue (forwardPressed));				//6
		networkPackage.AddValue (new NetworkPackageValue (packageNr));					//7

		if (Owner)
			packageNr++;
	}

	void Start()
	{
		if (NetworkManager.Me is null) Owner = true;
	}

	void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			int newNr = networkPackage.Value (7).GetInt32();

			if (newNr > packageNr)
			{
				_pitch = networkPackage.Value(1).GetFloat();
				_pitchSway = networkPackage.Value(2).GetFloat();
				_yaw = networkPackage.Value(2).GetFloat();
				_yawSway = networkPackage.Value(3).GetFloat();
				targetTransform.position = networkPackage.Value(4).GetVector3();
				targetTransform.rotation = networkPackage.Value(5).GetQuaternion();
				forwardPressed = networkPackage.Value(6).GetBool();
				packageNr = newNr;
			}
		}

		if (Owner)
		{
			UpdateForwardInput();
			UpdatePitch();
			UpdateYaw();
		}
    }

    void UpdateForwardInput()
    {
		forwardPressed = Input.GetKey(forwardKey);
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
