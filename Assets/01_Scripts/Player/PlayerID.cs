using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
	[SerializeField] short id;

	public short PlayerId { get { return id;} }
}
