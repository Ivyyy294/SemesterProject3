using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerStatus : NetworkBehaviour
{
	public enum StatusTyp
	{
		DEFAULT = 1 << 0,
		HAS_BALL = 1 << 1,
		LOW_OXYGEN = 1 << 2,
		EMPTY_OXYGEN = 1 << 3,
		STUNNED = 1 << 4,
	}

	ushort status = 0;

	//Public 
	//Sets StatusTyp flag
	public void SetStatusTyp (StatusTyp statusTyp)
	{
		ushort code = (ushort) statusTyp;
		status |= code;
	}

	public void SetStatusTyp (StatusTyp statusTyp, bool val)
	{
		if (val)
			SetStatusTyp (statusTyp);
		else
			RemoveStatusTyp (statusTyp);
	}

	//Deletes StatusTyp flag
	public void RemoveStatusTyp (StatusTyp statusTyp)
	{
		ushort code = (ushort) statusTyp;
		status ^= code;
	}

	//Return true if StatusTyp flag is set
	public bool CheckStatusTyp (StatusTyp statusTyp)
	{
		ushort code = (ushort) statusTyp;
		int result = status & code;
		return result == code;
	}

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue(new NetworkPackageValue(status));
	}

	//Private
	private void Start()
	{
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
	}

	// Update is called once per frame
	void Update()
    {
		if (!Owner && networkPackage.Available)
			status = networkPackage.Value(0).GetUShort();
    }
}
