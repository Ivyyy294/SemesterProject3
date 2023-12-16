using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
	[SerializeField] GameObject waypoint;
	[SerializeField] float smoothTime = 0.1F;

	private Vector3 velocityPosition;
	private Vector3 velocityForward;

    // Update is called once per frame
    void Update()
    {
		if (waypoint)
		{
			transform.position = Vector3.SmoothDamp(transform.position, waypoint.transform.position, ref velocityPosition, smoothTime);
			transform.forward = Vector3.SmoothDamp (transform.forward, waypoint.transform.forward, ref velocityForward, smoothTime);
		}
    }
}
