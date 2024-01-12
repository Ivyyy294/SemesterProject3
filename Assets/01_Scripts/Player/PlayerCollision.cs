using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	PlayerConfigurationContainer playerConfigurationContainer;
	PlayerInput diverInput;
	PlayerBlock playerBlock;

	public short PlayerId => playerConfigurationContainer.PlayerID;
	public PlayerOxygen PlayerOxygen => playerOxygen;
	
	public bool CanCatchBall { get { return !playerOxygen.OxygenEmpty && (playerBlock.IsBlocking || !diverInput.DashPressed);} }

	private void Start()
	{
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>();
		diverInput = transform.parent.GetComponentInChildren <PlayerInput>();
		playerBlock = GetComponent<PlayerBlock>();
	}
}
