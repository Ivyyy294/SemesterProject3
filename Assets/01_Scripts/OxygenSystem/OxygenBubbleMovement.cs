using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class OxygenBubbleMovement : MonoBehaviour
{
	[Range (0f, 1f)]
	[SerializeField] float buoyancy = 1f;
	
	AudioPlayer audioPlayerSpawnBubble;
	
	Rigidbody m_rigidbody;
    //Vector3 velocity;
	
	//Public
	public Rigidbody Rigidbody => m_rigidbody;

	public void Spawn()
	{
 		Debug.Log("Spawn Bubble");

		if (!audioPlayerSpawnBubble)
			audioPlayerSpawnBubble = GetComponent<AudioPlayer>();

		audioPlayerSpawnBubble.Play();
	}

	// Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
		audioPlayerSpawnBubble = GetComponent<AudioPlayer>();
    }

	private void FixedUpdate()
	{
		if (!m_rigidbody.isKinematic)
			m_rigidbody.AddForce (Vector3.up * buoyancy);
	}
}
