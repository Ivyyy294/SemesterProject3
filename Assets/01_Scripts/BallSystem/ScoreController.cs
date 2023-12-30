using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using TMPro;

public class ScoreController : NetworkBehaviour
{
	[SerializeField] TextMeshProUGUI labelPointsTeam1;
	[SerializeField] TextMeshProUGUI labelPointsTeam2;
	[SerializeField] AudioAsset audioScore;

	private ushort pointsTeam1;
	private ushort pointsTeam2;

	public void AddScore (int teamIndex)
	{
		if (Owner)
		{
			if (teamIndex == 0)
				pointsTeam1++;
			else
				pointsTeam2++;

			PlayAudioScore();
		}
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (pointsTeam1));
		networkPackage.AddValue (new NetworkPackageValue (pointsTeam2));
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
			pointsTeam1 = networkPackage.Value(0).GetUShort();
			pointsTeam2 = networkPackage.Value(1).GetUShort();
			networkPackage.Clear();
		}

		labelPointsTeam1.text = "Team1: " + pointsTeam1;
		labelPointsTeam2.text = "Team2: " + pointsTeam2;
    }

	[RPCAttribute]
	void PlayAudioScore()
	{
		if (Host)
			InvokeRPC("PlayAudioScore");

		audioScore?.PlayOneShot();
	}
}
