using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedByCurrent : MonoBehaviour
{
	[Range (0f, 500f)]
	[SerializeField] float currentForce;

    public float CurrentForce => currentForce;
}
