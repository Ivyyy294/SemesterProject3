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

	//Public Methods
	public void Throw (Vector3 startPos, Vector3 force)
	{
		if (Host)
		{
			CurrentPlayerId = -1;
			transform.position = startPos;
			ball.SetActive (true);
			InvokeRPC ("SpawnBall");
			m_rigidbody.AddForce (force);
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
		if (!Owner && networkPackage.Available)
		{
			CurrentPlayerId = networkPackage.Value(0).GetShort();
			transform.position = networkPackage.Value (1).GetVector3();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (Owner)
		{
			PlayerID playerID = collision.gameObject.GetComponentInChildren <PlayerID>();

			if (playerID)
			{
				CurrentPlayerId = playerID.PlayerId;
				ball.SetActive (false);
				InvokeRPC ("DespawnBall");
			}
		}
		else if (networkPackage.Available)
			CurrentPlayerId = networkPackage.Value(0).GetShort();
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (CurrentPlayerId));
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
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
