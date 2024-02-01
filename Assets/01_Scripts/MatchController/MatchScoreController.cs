using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchScoreController : NetworkBehaviour
{
	[SerializeField] AudioAsset audioScorePoint;

	private ushort[] teamPoints = new ushort[2];
	private MatchSoftReset matchSoftReset;
	private int lastTeamToScore = -1;

	//Public Methods
	public ushort PointsTeam1 => teamPoints[0];
	public ushort PointsTeam2 => teamPoints[1];
	public int LastTeamToScore => lastTeamToScore;
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
		{
			PlayerAudioScorePoint();
			teamPoints[teamIndex]++;
			lastTeamToScore = teamIndex;
			matchSoftReset.Invoke();
		}
	}

	//Protected Methods
	protected override void SetPackageData()
	{
		//ToDo send as one byte array
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[0]));
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[1]));
		networkPackage.AddValue (new NetworkPackageValue (lastTeamToScore));
	}

	//Private Methods
	private void Start()
	{
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
		matchSoftReset = GetComponent<MatchSoftReset>();
		lastTeamToScore = -1;
	}

	// Update is called once per frame
	void Update()
    {
		if (networkPackage.Available)
		{
			ushort newPointsTeam1 = networkPackage.Value(0).GetUShort();
			ushort newPointsTeam2 = networkPackage.Value(1).GetUShort();
			lastTeamToScore = networkPackage.Value (2).GetInt32();

			if (newPointsTeam1 > teamPoints[0]
				|| newPointsTeam2 > teamPoints[1])
			{
				matchSoftReset.Invoke();
			}

			teamPoints[0] = newPointsTeam1;
			teamPoints[1] = newPointsTeam2;
			networkPackage.Clear();
		}
    }

	[RPCAttribute]
	void PlayerAudioScorePoint()
	{
		if (Owner)
			InvokeRPC("PlayerAudioScorePoint");

		audioScorePoint?.PlayOneShot();
	}
}
