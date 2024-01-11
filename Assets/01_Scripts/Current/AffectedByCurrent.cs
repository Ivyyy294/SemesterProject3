using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedByCurrent : MonoBehaviour
{
	public enum ForceTyp
	{
		PLAYER,
		BALL,
		OXYGEN
	}

	[SerializeField] ForceTyp forceTyp;

    public ForceTyp GetForceTyp {get{return forceTyp; } }
}
