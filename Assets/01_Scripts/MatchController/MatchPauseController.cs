using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchPauseController : NetworkBehaviour
{
	float timeScale;
	float startTimeScale;

	public bool IsMatchPaused => Time.timeScale == 0f;

	public void PauseMatch(bool val)
	{
		if (Owner)
			timeScale = val ? 0f : 1f;
	}

    // Start is called before the first frame update
    void Start()
    {
		startTimeScale = Time.timeScale;
		timeScale = startTimeScale;
        Owner = !NetworkManager.Me || NetworkManager.Me.Host;
    }

    // Update is called once per frame
    void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			timeScale = networkPackage.Value(0).GetFloat();
			networkPackage.Clear();			
		}

		if (Time.timeScale != timeScale)
			Time.timeScale = timeScale;
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (timeScale));
	}

	private void OnDestroy()
	{
		Time.timeScale = startTimeScale;
	}
}
