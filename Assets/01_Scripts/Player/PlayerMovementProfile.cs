using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewPlayerMovementProfil", menuName = "PlayerMovementProfil")]
public class PlayerMovementProfil : ScriptableObject
{
	[Header ("Movement Settings")]

	[Min(0f)]
	public float maxSpeed = 1f;

	[Min(0f)]
	public float turnSpeedDegrees = 10f;

	[Range (0.3f, 3f)]
	public float movementSmoothTime = 0.3f;

	//[Header ("Physic Settings")]
	//[SerializeField] float drag = 1f;
	//[SerializeField] float angularDrag = 1f;
}
