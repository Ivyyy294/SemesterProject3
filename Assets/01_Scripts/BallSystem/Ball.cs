using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.Events;
using Ivyyy.Utils;

[RequireComponent (typeof (Rigidbody))]
public class Ball : NetworkBehaviour
{
	public static Ball Me { get; private set;}
	public short CurrentPlayerId {get; private set;}

	Rigidbody m_rigidbody;
	Vector3 velocity;
	float timer = 0f;
	
	[HideInInspector] public UnityEvent<Vector3> onBallThrown;
	[HideInInspector] public UnityEvent onBallCollided;

	//ToDo OnBallcaught

	[Header("Ball Settings")]
	[Range (0f, 10f)]
	[SerializeField] float drag = 1f;
	[SerializeField] float gravity = 0f;
	[SerializeField] float afterThrowCooldown = 0.5f;

	[Header ("Lara Values")]
	[SerializeField] GameObject ball;

	//Public Methods
	public Vector3 Velocity => velocity;
	public bool IsCatchable => (Owner && timer >= afterThrowCooldown && !IsCatched);
	public bool IsCatched => CurrentPlayerId != -1;

	public void Throw (Vector3 startPos, Vector3 force)
	{
		if (Host)
		{
			BallDrop (startPos);
			timer = 0f;
			m_rigidbody.AddForce (force);
			OnBallThrown (SerializationHelper.Vector3ToBytes(force));
		}
	}

	public void BallDrop (Vector3 spawnPos)
	{
		timer = 0f;
		CurrentPlayerId = -1;
		m_rigidbody.MovePosition (spawnPos);
		//transform.position = spawnPos;
		m_rigidbody.isKinematic = !Owner;
	}

	public void SetPlayerId (short playerId)
	{
		CurrentPlayerId = playerId;
	}

	//RPC
	[RPCAttribute]
	public void OnBallThrown(byte[] data)
	{
		if (Host)
			InvokeRPC("OnBallThrown", data);

		onBallThrown.Invoke(SerializationHelper.BytesToVector3 (data));
	}

	[RPCAttribute]
	public void OnBallCollided()
	{
		if (Host)
			InvokeRPC("OnBallCollided");

		onBallCollided.Invoke();
	}

	//Protected Methods
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (CurrentPlayerId));
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
		networkPackage.AddValue (new NetworkPackageValue (m_rigidbody.velocity));
	}

	//Private Methods
	// Start is called before the first frame update
	private void Awake()
	{
		if (Me == null)
			Me = this;
		else
			Destroy (gameObject);
	}

	void Start()
    {
        Owner = !NetworkManager.Me || NetworkManager.Me.Host;
		CurrentPlayerId = -1;
		m_rigidbody = GetComponent <Rigidbody>();
		m_rigidbody.isKinematic = !Owner;
		//m_rigidbody.drag = drag;
    }

	private void Update()
	{
		if (!Owner)
			UpdateClient();
		else
			UpdateHost();

		SetPhysicOptions();
	}

	private void OnCollisionEnter(Collision collision)
	{
		var force = collision.impulse.magnitude;

		if (force < 0.3)
			return;

		OnBallCollided();
	}

	private void SetPhysicOptions()
	{
		m_rigidbody.drag = drag;
		m_rigidbody.angularDrag = drag;
		m_rigidbody.useGravity = gravity!= 0f;
		Physics.gravity = Vector3.down * gravity;
	}

	private void UpdateClient()
	{
		if (networkPackage.Available)
		{
			CurrentPlayerId = networkPackage.Value(0).GetShort();

			if (!IsCatched)
			{
				transform.position = networkPackage.Value (1).GetVector3();
				velocity = networkPackage.Value(2).GetVector3();
			}
			
			networkPackage.Clear();
		}
		else if (!IsCatched)
			transform.position += velocity * Time.deltaTime;
	}

	private void UpdateHost()
	{
		m_rigidbody.isKinematic = IsCatched;

		if (timer < afterThrowCooldown)
			timer += Time.deltaTime;

		velocity = m_rigidbody.velocity;
	}
}
