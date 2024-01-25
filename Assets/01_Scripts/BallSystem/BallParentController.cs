using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallParentController : MonoBehaviour
{
	Ball ball;
	Rigidbody mrigidbody;

    // Start is called before the first frame update
    void Start()
    {
        ball = GetComponent<Ball>();
		mrigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.CurrentPlayerId == -1 && transform.parent != null)
			transform.SetParent (null);
		else if (ball.CurrentPlayerId != -1 && transform.parent == null)
			transform.SetParent (GetParentTransform());
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
