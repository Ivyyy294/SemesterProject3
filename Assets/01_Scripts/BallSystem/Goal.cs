using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class Goal : MonoBehaviour
{
	[SerializeField] GameEvent teamScoredEvent;

	private void OnCollisionEnter(Collision collision)
	{
		Ball ball = collision.gameObject.GetComponent<Ball>();

		if (ball && ball.Owner)
		{
			ball.gameObject.SetActive (false);
			teamScoredEvent.Raise();
		}
	}
}
