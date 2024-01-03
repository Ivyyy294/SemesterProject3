using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (Rigidbody))]
public class OxygenMovement : NetworkBehaviour
{
	[Range (0f, 1f)]
	[SerializeField] float buoyancy = 1f;
	
	Rigidbody m_rigidbody;
    //Vector3 velocity;
	
	// Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
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
		m_rigidbody.MovePosition (transform.position + (Vector3.up * buoyancy * Time.fixedDeltaTime));
	}
}
