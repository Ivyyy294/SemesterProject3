using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using Ivyyy.GameEvent;
using System;

public class MatchScoreController : NetworkBehaviour
{
	[SerializeField] AudioAsset audioScorePoint;
	[SerializeField] GameEvent playerScored;

	private ushort[] teamPoints = new ushort[2];
	private MatchSoftReset matchSoftReset;
	private int lastTeamToScore = -1;

	private int lastPlayerIndex = -1;
	private int lastScoringPlayer = -1;
	private int[] playerScoreCount = new int[4];

	//Public Methods
	public ushort PointsTeam1 => teamPoints[0];
	public ushort PointsTeam2 => teamPoints[1];
	public int LastTeamToScore => lastTeamToScore;
	public int LastScoringPlayer => lastScoringPlayer;
	public int [] PlayerScoreCount => playerScoreCount;

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
			lastScoringPlayer = lastPlayerIndex;
			teamPoints[teamIndex]++;
			lastTeamToScore = teamIndex;
			OnPointScored();
		}
	}

	//Protected Methods
	protected override void SetPackageData()
	{
		//ToDo send as one byte array
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[0]));
		networkPackage.AddValue (new NetworkPackageValue (teamPoints[1]));
		networkPackage.AddValue (new NetworkPackageValue (lastTeamToScore));
		networkPackage.AddValue (new NetworkPackageValue (lastScoringPlayer));

		byte[] playerScoreCountBytes = new byte[playerScoreCount.Length * sizeof(int)];
		Buffer.BlockCopy(playerScoreCount, 0, playerScoreCountBytes, 0, playerScoreCountBytes.Length);

		networkPackage.AddValue (new NetworkPackageValue (playerScoreCountBytes));
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
			lastScoringPlayer = networkPackage.Value (3).GetInt32();

			if (newPointsTeam1 > teamPoints[0]
				|| newPointsTeam2 > teamPoints[1])
			{
				OnPointScored();
			}

			teamPoints[0] = newPointsTeam1;
			teamPoints[1] = newPointsTeam2;

			byte[] playerScoreCountBytes = networkPackage.Value(4).GetBytes();
			Buffer.BlockCopy(playerScoreCountBytes, 0, playerScoreCount, 0, playerScoreCountBytes.Length);

			networkPackage.Clear();
		}
		else if (Owner && Ball.Me && Ball.Me.CurrentPlayerId != -1)
			lastPlayerIndex = Ball.Me.CurrentPlayerId;
    }

	void OnPointScored()
	{
		audioScorePoint?.PlayOneShot();
			
		if (lastScoringPlayer != -1)
			playerScoreCount[lastScoringPlayer]++;

		matchSoftReset.Invoke();
		playerScored?.Raise();
	}
}
