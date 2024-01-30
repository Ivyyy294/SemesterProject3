using System;
using UnityEngine;
using Ivyyy.Network;
using Ivyyy.Utils;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputProcessing : NetworkBehaviour
{
	// [Header("Key bindings")]
	// [SerializeField] KeyCode forwardKey = KeyCode.Space;
	// [SerializeField] KeyCode dashKey = KeyCode.Mouse1;
	// [SerializeField] KeyCode throwKey = KeyCode.Mouse0;
	// [SerializeField] KeyCode blockKey = KeyCode.B;
	// [SerializeField] KeyCode stealKey = KeyCode.Mouse0;

	private float _pitch;
    private float _yaw;
    private bool _goingForward;
    private bool _isDashing;
    private bool _isThrowing;
    private bool _isBlocking;
    private bool _isStealing;

    public float Pitch => _pitch;
    public float Yaw => _yaw;
	public bool ForwardPressed {get{ return inputBuffer.Check (0);}}
	public bool DashPressed {get{ return inputBuffer.Check (1);}}
	public bool BlockPressed { get { return inputBuffer.Check(2);} }
	// public bool ThrowPressed { get { return Input.GetKeyDown (throwKey);} }
	public bool ThrowPressed => _isThrowing;
	public bool StealPressed { get { return inputBuffer.Check(3);} }

	Transform targetTransform;
	int packageNr = 0;
	BitSet inputBuffer = new BitSet (1);

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
		if (NetworkManager.Me is null)
		{
			Owner = true;
		}
		else
			Owner = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().IsLocalPlayer();

		targetTransform = transform.parent;
		
		GetComponent<PlayerInput>().enabled = Owner;
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
			// inputBuffer.SetBit (0, Input.GetKey(forwardKey));
			inputBuffer.SetBit(0, _goingForward);
			// inputBuffer.SetBit (1, Input.GetKey (dashKey));
			inputBuffer.SetBit(1, _isDashing);
			// inputBuffer.SetBit (2, Input.GetKey (blockKey));
			inputBuffer.SetBit(2, _isBlocking);
			// inputBuffer.SetBit (3, Input.GetKey (stealKey));
			inputBuffer.SetBit(3, _isStealing);
			// _pitch = Input.GetAxisRaw("Vertical");
			// _yaw = Input.GetAxisRaw("Horizontal");
		}
    }

	private void OnDisable()
	{
		packageNr = 0;
		networkPackage.Clear();
	}
	
	#region InputCallbacks

	public void OnTurn(InputAction.CallbackContext context)
	{
		var v = context.ReadValue<Vector2>();
		_pitch = v.y;
		_yaw = v.x;
	}

	public void OnSwim(InputAction.CallbackContext context)
	{
		_goingForward = context.ReadValue<float>() > 0.5;
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		_isDashing = context.ReadValue<float>() > 0.5;
	}

	public void OnThrow(InputAction.CallbackContext context)
	{
		if (context.started) _isThrowing = true;
		if (context.canceled) _isThrowing = false;
	}

	public void OnBlock(InputAction.CallbackContext context)
	{
		_isBlocking = context.performed;
	}

	public void OnSteal(InputAction.CallbackContext context)
	{
		_isStealing = context.performed;
	}
	
	#endregion
	
}
