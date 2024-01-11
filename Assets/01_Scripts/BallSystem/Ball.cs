using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (Rigidbody))]
public class Ball : NetworkBehaviour
{
	public static Ball Me { get; private set;}
	public short CurrentPlayerId {get; private set;}

	Rigidbody m_rigidbody;
	Vector3 velocity;
	float timer = 0f;
	
	[Header("Ball Settings")]
	[Range (0f, 10f)]
	[SerializeField] float drag = 1f;
	[SerializeField] float gravity = 0f;
	[SerializeField] float afterThrowCooldown = 0.5f;

	[Header ("Lara Values")]
	[SerializeField] GameObject ball;

	//Public Methods
	public void Throw (Vector3 startPos, Vector3 force)
	{
		if (Host)
		{
			timer = 0f;
			BallDrop (startPos);
			m_rigidbody.AddForce (force);
		}
	}

	public void BallDrop (Vector3 spawnPos)
	{
		timer = 0f;
		CurrentPlayerId = -1;
		transform.position = spawnPos;
		ball.SetActive (true);
		InvokeRPC ("SpawnBall");
	}

	//Protected Methods
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (CurrentPlayerId));
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
		networkPackage.AddValue (new NetworkPackageValue (m_rigidbody.velocity));
	}

	[RPCAttribute]
	protected void DespawnBall()
	{
		Debug.Log ("DespawnBall");
		ball.SetActive (false);
	}

	[RPCAttribute]
	protected void SpawnBall()
	{
		Debug.Log ("SpawnBall");
		ball.SetActive (true);
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
		{
			if (networkPackage.Available)
			{
				CurrentPlayerId = networkPackage.Value(0).GetShort();
				transform.position = networkPackage.Value (1).GetVector3();
				velocity = networkPackage.Value(2).GetVector3();
				networkPackage.Clear();
			}
			else
				transform.position += velocity * Time.deltaTime;
		}
		else if (timer < afterThrowCooldown)
			timer += Time.deltaTime;

		SetPhysicOptions();
	}

	//ToDo Move to PlayerCollision
	private void OnTriggerEnter(Collider other)
	{
		//Prevent ball from being catch during cooldown
		if (Owner && timer >= afterThrowCooldown)
		{
			PlayerCollision playerCollision = other.GetComponentInParent<PlayerCollision>();
			
			if (playerCollision && playerCollision.CanCatchBall)
				Catch (playerCollision.PlayerId);
		}
	}

	private void Catch (short playerId)
	{
		CurrentPlayerId = playerId;
		ball.SetActive(false);
		InvokeRPC("DespawnBall");
	}

	private void SetPhysicOptions()
	{
		m_rigidbody.drag = drag;
		m_rigidbody.angularDrag = drag;
		m_rigidbody.useGravity = gravity!= 0f;
		Physics.gravity = Vector3.down * gravity;
	}
}
