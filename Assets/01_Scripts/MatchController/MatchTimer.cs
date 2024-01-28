using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class MatchTimer : NetworkBehaviour
{
	[Range (0.5f, 10f)]
	[SerializeField] float matchLengthMinutes= 5f;

	float matchLengthSeconds;
	float timer;
	MatchPauseController matchPauseController;

	public float TimeRemaining { get { return Mathf.Max (0f, matchLengthSeconds - timer);}}

    // Start is called before the first frame update
    void Start()
    {
		//Convert to minutes to ms
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
        matchLengthSeconds = matchLengthMinutes * 60;
		matchPauseController = MatchController.Me.MatchPauseController;
    }

    // Update is called once per frame
    void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			timer = networkPackage.Value (0).GetFloat();
			networkPackage.Clear();
		}
		else if (timer <= matchLengthSeconds && !matchPauseController.IsMatchPaused)
			timer += Time.deltaTime;
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue(new NetworkPackageValue (timer));
	}
}
