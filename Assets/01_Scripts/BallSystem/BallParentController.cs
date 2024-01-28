using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallParentController : MonoBehaviour
{
	Ball ball;
	Rigidbody mrigidbody;
	Transform defaultParent;

    // Start is called before the first frame update
    void Start()
    {
        ball = GetComponent<Ball>();
		mrigidbody = GetComponent<Rigidbody>();
		defaultParent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.CurrentPlayerId == -1 && transform.parent != defaultParent)
			transform.SetParent (defaultParent);
		else if (ball.CurrentPlayerId != -1 && transform.parent == defaultParent)
		{
			Transform newParent = GetParentTransform();
			transform.SetParent (newParent);
			transform.forward = newParent.forward;
			transform.localPosition = Vector3.zero;
		}
    }

	Transform GetParentTransform()
	{
		foreach (PlayerHoldBall i in PlayerHoldBall.InstanzList)
		{
			if (i.gameObject.activeInHierarchy && i.PlayerId == ball.CurrentPlayerId)
				return i.BallHoldTransform;
		}

		return null;
	}
}
