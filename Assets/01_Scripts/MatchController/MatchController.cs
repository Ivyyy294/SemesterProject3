using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
	public static MatchController Me { get; private set;}

	public MatchTimer MatchTimer {get; private set;}
	public MatchScoreController MatchScoreController {get; private set;}
	public MatchGameOver MatchGameOver { get; private set;}

    // Start is called before the first frame update
    void Awake ()
    {
        if (Me == null)
		{
			Me = this;
			MatchTimer = GetComponent<MatchTimer>();
			MatchScoreController = GetComponent<MatchScoreController>();
			MatchGameOver = GetComponent <MatchGameOver>();
		}
		else
			Destroy (gameObject);
    }
}
