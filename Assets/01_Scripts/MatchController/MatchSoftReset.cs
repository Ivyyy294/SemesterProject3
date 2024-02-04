using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using Ivyyy.GameEvent;

public class MatchSoftReset : NetworkBehaviour
{
	private MatchTimer matchTimer;
	private MatchPauseController pauseController;
	private MatchObjectSpawn objectSpawnController;
	float timer = 0f;
	bool delayDone = false;

	[SerializeField] AudioAsset audioRoundStart;
	[SerializeField] float pauseTime;
	[SerializeField] GameEvent resetEvent;
	[SerializeField] GameEvent showBubbleEvent;
	[SerializeField] float softResetDelay = 1f;

	public float PauseTimeRemaining => Mathf.Max (0f, pauseTime - timer);

	public void Invoke()
	{
		if (matchTimer.TimeRemaining > 0f)
		{
			showBubbleEvent?.Raise();
			StartCoroutine (SoftResetDelay());
		}
	}

	protected override void SetPackageData()
	{
		networkPackage.AddValue(new NetworkPackageValue (timer));
	}

	// Start is called before the first frame update
	void Start()
    {
		timer = pauseTime;
        pauseController = GetComponent<MatchPauseController>();
		objectSpawnController = GetComponent<MatchObjectSpawn>();
		matchTimer = GetComponent <MatchTimer>();

		SoftReset (false);
		showBubbleEvent?.Raise();
    }

    // Update is called once per frame
    void Update()
    {
		if (!MatchController.Me.MatchGameOver.GameOver())
		{
			if (!Owner && networkPackage.Available)
			{
				timer = networkPackage.Value(0).GetFloat();
				networkPackage.Clear();
			}
			if (timer < pauseTime)
				timer += Time.unscaledDeltaTime;
			else if (pauseController.IsMatchPaused && delayDone)
			{
				audioRoundStart?.Play();
				pauseController.PauseMatch (false);
			}
		}
    }

	void SoftReset(bool pauseGame = true)
	{
		objectSpawnController.RespawnObjects();

		if (pauseTime > 0f && pauseGame)
		{
			pauseController.PauseMatch (true);
			timer = 0f;
		}

		//audioRoundStart?.Play();

		resetEvent?.Raise();
	}

	IEnumerator SoftResetDelay()
	{
		delayDone = false;
		float delayTimer = 0f;

		while (delayTimer < softResetDelay)
		{
			delayTimer += Time.deltaTime;
			yield return null;
		}

		delayDone = true;
		SoftReset();
	}
}
