using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class Goal : MonoBehaviour
{
	[SerializeField] int teamIndex;
	[SerializeField] GameEvent team1Scored;
	[SerializeField] GameEvent team2Scored;

	private void OnCollisionEnter(Collision collision)
	{
		Ball ball = collision.gameObject.GetComponent<Ball>();

		if (ball && ball.Owner)
		{
			if (teamIndex == 0)
				team1Scored.Raise();
			else
				team2Scored.Raise();
		}
	}
}
