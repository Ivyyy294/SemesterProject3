using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchScoreController : NetworkBehaviour
{
	private ushort[] teamPoints = new ushort[2];

	public ushort PointsTeam1 => teamPoints[0];
	public ushort PointsTeam2 => teamPoints[1];

	public bool Tie {get{return PointsTeam1 == PointsTeam2; } }

	public bool HasTeamWon (int teamIndex)
	{
		if (teamIndex == 0)
			return PointsTeam1 > PointsTeam2;
		else
			return PointsTeam2 > PointsTeam1;
	}

	public void AddScore (int teamIndex)
	{
		if (Owner)
			teamPoints[teamIndex]++;
	}

	protected override void SetPackageData()
	{
		//ToDo send as one byte array
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[0]));
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[1]));
	}

	private void Start()
	{
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
	}

	// Update is called once per frame
	void Update()
    {
		if (networkPackage.Available)
		{
			teamPoints[0] = networkPackage.Value(0).GetUShort();
			teamPoints[1] = networkPackage.Value(1).GetUShort();
			networkPackage.Clear();
		}
    }
}
