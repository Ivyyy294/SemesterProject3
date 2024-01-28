using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatch : MonoBehaviour
{
	[Header("Catch Settings")]
	[Range (0.1f, 1f)]
	[SerializeField] float stealRange = 0.5f;
	[SerializeField] LayerMask mask = -1;
	[SerializeField] Transform checkPosition;

	PlayerOxygen playerOxygen;
	PlayerConfigurationContainer playerConfigurationContainer;
	PlayerAudio playerAudio;

	public bool CanCatchBall { get { return !playerOxygen.OxygenEmpty && Ball.Me.IsCatchable;;} }

    // Start is called before the first frame update
    void Start()
    {
        playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>();
		playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
    }

	bool IsBallInRange()
	{
		//Layer ball collider
		Collider[] colliders = Physics.OverlapSphere (checkPosition.position, stealRange, mask.value);

		if (colliders.Length > 0)
			return true;

		return false;
	}

	void Catch()
	{
		if (Ball.Me)
		{
			Ball.Me.SetPlayerId (playerConfigurationContainer.PlayerID);
			playerAudio.PlayAudioCatch();
		}
		else
			Debug.Log ("No active ball!");
	}

	private void FixedUpdate()
	{
		if (CanCatchBall && IsBallInRange())
			Catch();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (checkPosition.position, stealRange);
	}
}
