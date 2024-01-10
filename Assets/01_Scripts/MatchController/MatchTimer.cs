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

	public float TimeRemaining { get { return Mathf.Max (0f, matchLengthSeconds - timer);}}
	public bool GameOver { get { return TimeRemaining == 0f;} }

    // Start is called before the first frame update
    void Start()
    {
		//Convert to minutes to ms
        matchLengthSeconds = matchLengthMinutes * 60;
    }

    // Update is called once per frame
    void Update()
    {
		if (!Owner && networkPackage.Available)
			timer = networkPackage.Value (0).GetFloat();
		else if (timer <= matchLengthSeconds)
			timer += Time.deltaTime;

		if (GameOver)
			Time.timeScale = 0f;
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue(new NetworkPackageValue (timer));
	}
}
