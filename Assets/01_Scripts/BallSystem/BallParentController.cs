using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallParentController : MonoBehaviour
{
	Ball ball;
	Transform defaultParent;

	public void ResetParent()
	{
		transform.SetParent (defaultParent);
	}

    // Start is called before the first frame update
    void Start()
    {
        ball = GetComponent<Ball>();
		defaultParent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.CurrentPlayerId == -1 && transform.parent != defaultParent)
			ResetParent();
		else if (ball.CurrentPlayerId != -1)
		{
			Transform newParent = GetParentTransform();

			if (transform.parent != newParent)
			{
				transform.SetParent (newParent);
				// transform.forward = newParent.forward;
				transform.localRotation = Quaternion.identity;
				transform.localPosition = Vector3.zero;
			}
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
