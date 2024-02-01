using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

//[RequireComponent (typeof (PlayerBallStatus))]
public class PlayerOxygen : NetworkBehaviour
{
	[Range (1f, 1000f)]
	[SerializeField] float maxOxygen = 100f;

	[Range (0f, 100f)]
	[SerializeField] float passiveOxygenConsumption = 10f;

	[Range (0f, 100f)]
	[SerializeField] float dashOxygenConsumption = 10f;
	
	[Header ("Round start refill")]
	[Range (0, 1)]
	[SerializeField] float oxygenRefillPercentTeamScored = 0.5f;
	[Range (0, 1)]
	[SerializeField] float oxygenRefillPercentTeamNotScored = 1f;

	float currentOxygen;
	PlayerInputProcessing playerInput;
	PlayerAudio playerAudio;
	PlayerConfigurationContainer playerConfigurationContainer;

	//Public
	public bool OxygenEmpty { get {return currentOxygen <= 0f;} }
	public float CurrentOxygen => currentOxygen;
	public float CurrentOxygenPercent {get{return (currentOxygen / maxOxygen) * 100;} }
	public float MissingOxygen { get {return maxOxygen - currentOxygen;} }

	public void Refill (float val)
	{
		if (currentOxygen < maxOxygen)
			currentOxygen = Mathf.Min (currentOxygen + val, maxOxygen);
	}
	
	public void RefillMax()
	{
		currentOxygen = maxOxygen;
	}

	public void RefillRoundStart()
	{
		if (MatchController.Me)
		{
			if (MatchController.Me.MatchScoreController.LastTeamToScore == playerConfigurationContainer.TeamIndex)
				currentOxygen = maxOxygen * oxygenRefillPercentTeamScored;
			else
				currentOxygen = maxOxygen * oxygenRefillPercentTeamNotScored;
		}
		else
			currentOxygen = maxOxygen * oxygenRefillPercentTeamNotScored;
	}

	public void PlayerAudioInhale()
	{
		playerAudio.PlayAudioInhale();
	}

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (currentOxygen));
	}

	//Private
	// Start is called before the first frame update
	void Awake()
	{
		currentOxygen = maxOxygen;
		playerInput = transform.parent.GetComponentInChildren<PlayerInputProcessing>();
		playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
		playerConfigurationContainer = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>();
	}

	// Update is called once per frame
	void Update()
    {
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;

		if (!Owner && networkPackage.Available)
			currentOxygen = networkPackage.Value(0).GetFloat();

		//Prevent oxygen loss while game is paused
		if (MatchController.Me != null && MatchController.Me.MatchPauseController.IsMatchPaused)
			return;

		if (currentOxygen > 0f)
			currentOxygen -= passiveOxygenConsumption * Time.deltaTime;

		if (playerInput.DashPressed)
			currentOxygen -= dashOxygenConsumption * Time.deltaTime;
    }
}
