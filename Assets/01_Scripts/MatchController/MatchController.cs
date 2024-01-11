using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
	public static MatchController Me { get; private set;}

	public MatchTimer MatchTimer {get; private set;}
	public MatchScoreController MatchScoreController {get; private set;}
	public MatchGameOver MatchGameOver { get; private set;}
	public MatchPauseController MatchPauseController { get; private set;}
	public MatchSoftReset MatchSoftReset { get; private set;}

    // Start is called before the first frame update
    void Awake ()
    {
        if (Me == null)
		{
			Me = this;
			MatchTimer = GetComponent<MatchTimer>();
			MatchScoreController = GetComponent<MatchScoreController>();
			MatchGameOver = GetComponent <MatchGameOver>();
			MatchPauseController = GetComponent <MatchPauseController>();
			MatchSoftReset = GetComponent<MatchSoftReset>();
		}
		else
			Destroy (gameObject);
    }

	private void OnDestroy()
	{
		if (Me == this)
			Me = null;
	}
}
