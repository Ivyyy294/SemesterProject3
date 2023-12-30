using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class TestWaypoint : NetworkBehaviour
{
	[SerializeField] float speed;
	float timeCounter = 0f;
	Vector3 startPos;
	Vector3 startForward;

    // Start is called before the first frame update
    void Start()
    {
        Owner = !NetworkManager.Me || NetworkManager.Me.Host;
		startPos = transform.position;
		startForward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Owner && networkPackage.Count > 0)
		{
			transform.position = networkPackage.Value(0).GetVector3();
			transform.forward = networkPackage.Value(1).GetVector3();
		}
		else
		{
			// Increment the time counter
			timeCounter += Time.deltaTime;

			// Calculate the new Y position using the sine function
			float newY = Mathf.Sin(timeCounter * speed) * speed;
			transform.position = new Vector3 (startPos.x, startPos.y + newY, startPos.z);
			transform.forward = new Vector3 (startForward.x,  startForward.y + newY, startForward.z);
		}
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
		networkPackage.AddValue (new NetworkPackageValue (transform.forward));
	}
}
