using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchSoftReset : NetworkBehaviour
{
	private MatchPauseController pauseController;
	private MatchObjectSpawn objectSpawnController;
	float timer = 0f;

	[SerializeField] float pauseTime;

	public float PauseTimeRemaining => Mathf.Max (0f, pauseTime - timer);

	public void Invoke()
	{
		objectSpawnController.RespawnObjects();
		pauseController.PauseMatch (true);
		timer = 0f;
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
    }

    // Update is called once per frame
    void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			timer = networkPackage.Value(0).GetFloat();
			networkPackage.Clear();
		}
        if (timer < pauseTime)
			timer += Time.unscaledDeltaTime;
		else if (pauseController.IsMatchPaused)
			pauseController.PauseMatch (false);
    }
}
