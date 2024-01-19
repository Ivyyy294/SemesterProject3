using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	PlayerConfigurationContainer playerConfigurationContainer;
	PlayerInput diverInput;
	PlayerBlock playerBlock;
	PlayerBallStatus playerBallStatus;

	public short PlayerId => playerConfigurationContainer.PlayerID;
	public PlayerOxygen PlayerOxygen => playerOxygen;
	public PlayerBallStatus PlayerBallStatus => playerBallStatus;

	public bool CanCatchBall { get { return !playerOxygen.OxygenEmpty/* && (playerBlock.IsBlocking || !diverInput.DashPressed)*/;} }

	private void Start()
	{
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>();
		diverInput = transform.parent.GetComponentInChildren <PlayerInput>();
		playerBlock = GetComponent<PlayerBlock>();
		playerBallStatus = transform.parent.GetComponentInChildren <PlayerBallStatus>();
	}
}
