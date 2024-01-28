using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchPauseController : NetworkBehaviour
{
	//float timeScale;
	//float startTimeScale;

	public bool IsMatchPaused => paused;
	
	bool paused = false;

	public void PauseMatch(bool val)
	{
		if (Owner)
			paused = val;
	}

    // Start is called before the first frame update
    void Start()
    {
        Owner = !NetworkManager.Me || NetworkManager.Me.Host;
    }

    // Update is called once per frame
    void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			paused = networkPackage.Value(0).GetBool();
			networkPackage.Clear();			
		}
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (paused));
	}
}
