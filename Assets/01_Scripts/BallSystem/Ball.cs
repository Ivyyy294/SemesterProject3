using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (Rigidbody))]
public class Ball : NetworkBehaviour
{
	public static Ball Me { get; private set;}
	public short CurrentPlayerId {get; private set;}

	[SerializeField] GameObject ball;
	Rigidbody m_rigidbody;
	Vector3 velocity;

	//Public Methods
	public void Throw (Vector3 startPos, Vector3 force)
	{
		if (Host)
		{
			BallDrop (startPos);
			m_rigidbody.AddForce (force);
		}
	}

	public void BallDrop (Vector3 spawnPos)
	{
		CurrentPlayerId = -1;
		transform.position = spawnPos;
		ball.SetActive (true);
		InvokeRPC ("SpawnBall");
	}

	public void RespawnBall()
	{
		if (Host)
		{
			transform.position = Vector3.zero;
			m_rigidbody.velocity = Vector3.zero;
		}
	}

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
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Owner)
		{
			PlayerCollision playerCollision = other.GetComponentInParent<PlayerCollision>();
			
			if (playerCollision)
			{
				PlayerID playerID = other.gameObject.GetComponentInParent<PlayerID>();
				PlayerOxygen playerOxygen = playerCollision.PlayerOxygenSystem.GetComponent<PlayerOxygen>();

				if (playerID && playerOxygen && !playerOxygen.OxygenEmpty)
				{
					CurrentPlayerId = playerID.PlayerId;
					ball.SetActive(false);
					InvokeRPC("DespawnBall");
				}
			}
		}
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (CurrentPlayerId));
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
		networkPackage.AddValue (new NetworkPackageValue (m_rigidbody.velocity));
	}

	[RPCAttribute]
	public void DespawnBall()
	{
		ball.SetActive (false);
	}

	[RPCAttribute]
	public void SpawnBall()
	{
		ball.SetActive (true);
	}
}
