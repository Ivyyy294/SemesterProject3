using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	PlayerOxygen playerOxygen;
	PlayerConfigurationContainer playerConfigurationContainer;
	PlayerInput diverInput;

	public short PlayerId => playerConfigurationContainer.PlayerID;
	public bool CanCatchBall { get { return !playerOxygen.OxygenEmpty && !diverInput.DashPressed;} }
	public PlayerOxygen PlayerOxygen => playerOxygen;

	private void Start()
	{
		playerOxygen = transform.parent.GetComponentInChildren<PlayerOxygen>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren <PlayerConfigurationContainer>();
		diverInput = transform.parent.GetComponentInChildren <PlayerInput>();
	}
}
