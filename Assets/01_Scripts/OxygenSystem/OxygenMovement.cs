using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

[RequireComponent (typeof (Rigidbody))]
public class OxygenMovement : MonoBehaviour
{
	[Range (0f, 1f)]
	[SerializeField] float buoyancy = 1f;
	
	Rigidbody m_rigidbody;
    //Vector3 velocity;
	
	// Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

	//private void Update()
	//{
	//	if (!Owner)
	//	{
	//		if (networkPackage.Available)
	//		{
	//			transform.position = networkPackage.Value(0).GetVector3();
	//			velocity = networkPackage.Value (1).GetVector3();
	//		}

	//		transform.position += velocity * Time.deltaTime;
	//	}
	//}

	private void FixedUpdate()
	{
		//if (Owner)
		m_rigidbody.MovePosition (transform.position + (Vector3.up * buoyancy * Time.fixedDeltaTime));
	}
}
