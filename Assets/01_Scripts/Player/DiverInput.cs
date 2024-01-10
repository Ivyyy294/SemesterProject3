using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ivyyy.Network;
using Ivyyy.Utils;

public class DiverInput : NetworkBehaviour
{
    private float _pitch;
    private float _yaw;
	private bool forwardPressed = false;

    public float Pitch => _pitch;
    public float Yaw => _yaw;
	public bool ForwardPressed {get{ return inputBuffer.Check (0);}}
	public bool DashPressed {get{ return inputBuffer.Check (1);}}

	Transform targetTransform;
	int packageNr = 0;
	BitSet inputBuffer = new BitSet (1);
	PlayerConfigurationContainer playerConfigurationContainer;

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (_pitch));						//0
		networkPackage.AddValue (new NetworkPackageValue (_yaw));						//1
		networkPackage.AddValue (new NetworkPackageValue (targetTransform.position));	//2
		networkPackage.AddValue (new NetworkPackageValue (targetTransform.rotation));	//3
		networkPackage.AddValue (new NetworkPackageValue (inputBuffer.GetRawData()));	//4
		networkPackage.AddValue (new NetworkPackageValue (packageNr));					//5

		if (Owner)
			packageNr++;
	}

	void Start()
	{
		if (NetworkManager.Me is null) Owner = true;
		targetTransform = transform.parent;
		playerConfigurationContainer = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>();
	}

	void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			int newNr = networkPackage.Value (5).GetInt32();

			if (newNr > packageNr)
			{
				_pitch = networkPackage.Value(0).GetFloat();
				_yaw = networkPackage.Value(1).GetFloat();
				targetTransform.position = networkPackage.Value(2).GetVector3();
				targetTransform.rotation = networkPackage.Value(3).GetQuaternion();
				inputBuffer.SetRawData (networkPackage.Value(4).GetBytes());
				packageNr = newNr;
			}
		}

		if (Owner)
		{
			inputBuffer.SetBit (0, Input.GetKey(KeyCode.Space));
			inputBuffer.SetBit (1, Input.GetMouseButton (1));
			_pitch = Input.GetAxisRaw("Vertical");
			_yaw = Input.GetAxisRaw("Horizontal");
		}
    }

	private void OnDisable()
	{
		packageNr = 0;
		networkPackage.Clear();
	}
}
