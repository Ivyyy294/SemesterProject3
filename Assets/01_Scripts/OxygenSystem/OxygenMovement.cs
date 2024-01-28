using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (Rigidbody))]
public class OxygenMovement : NetworkBehaviour
{
	[Range (0f, 1f)]
	[SerializeField] float buoyancy = 1f;
	
	AudioPlayer audioPlayerSpawnBubble;
	
	Rigidbody m_rigidbody;
    //Vector3 velocity;
	
	//Public
	public void SpawnAt (Vector3 pos)
	{
		transform.position = pos;
		Spawn();
	}

	[RPCAttribute]
	public void Spawn()
	{
 		Debug.Log("Spawn Bubble");

		if (Owner)
			InvokeRPC ("Spawn");

		if (!audioPlayerSpawnBubble)
			audioPlayerSpawnBubble = GetComponent<AudioPlayer>();

		audioPlayerSpawnBubble.Play();
		gameObject.SetActive(true);
	}

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
	}

	// Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
		audioPlayerSpawnBubble = GetComponent<AudioPlayer>();
    }

	private void Update()
	{
		if (!Owner && networkPackage.Available)
		{
			transform.position = networkPackage.Value(0).GetVector3();
			networkPackage.Clear();
		}
	}

	private void FixedUpdate()
	{
		m_rigidbody.AddForce (Vector3.up * buoyancy);
	}
}
