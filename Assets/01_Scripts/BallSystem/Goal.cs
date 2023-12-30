using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
	[SerializeField] int teamIndex;
	[SerializeField] ScoreController scoreController;

	private void OnCollisionEnter(Collision collision)
	{
		Ball ball = collision.gameObject.GetComponent<Ball>();

		if (ball && ball.Owner)
		{
			ball.RespawnBall();
			scoreController.AddScore (teamIndex);
		}
	}
}
