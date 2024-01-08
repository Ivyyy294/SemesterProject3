using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	[SerializeField] GameObject playerOxygenSystem;
	[SerializeField] GameObject playerSettingSystem;
	public GameObject PlayerOxygenSystem => playerOxygenSystem;
	public GameObject PlayerSettingSystem => playerSettingSystem;
}
