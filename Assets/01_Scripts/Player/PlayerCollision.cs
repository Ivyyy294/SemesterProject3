using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	PlayerBallStatus playerBallStatus;
	PlayerCatch playerCatch;

	public PlayerOxygen PlayerOxygen => playerOxygen;
	public PlayerBallStatus PlayerBallStatus => playerBallStatus;
	public PlayerCatch PlayerCatch => playerCatch;
	public PlayerConfigurationContainer playerConfiguration;

	private void Start()
	{
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerCatch = transform.parent.GetComponentInChildren <PlayerCatch>();
		playerBallStatus = transform.parent.GetComponentInChildren <PlayerBallStatus>();
		playerConfiguration = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>();
	}
}
