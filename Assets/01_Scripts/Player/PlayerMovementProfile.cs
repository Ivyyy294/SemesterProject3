using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewPlayerMovementProfil", menuName = "PlayerMovementProfil")]
public class PlayerMovementProfil : ScriptableObject
{
	[Min(0f)]
	public float maxSpeed = 1f;
	[Min(0f)]
	public float turnSpeedDegrees = 10f;
	public AnimationCurve accelerationCurve;
	public AnimationCurve deaccelerationCurve;
}
