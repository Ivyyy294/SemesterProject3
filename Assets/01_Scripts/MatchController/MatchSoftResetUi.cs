using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSoftResetUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI labelTimer;
	MatchSoftReset matchSoftReset;

    // Start is called before the first frame update
    void Start()
    {
        matchSoftReset = MatchController.Me.MatchSoftReset;
		labelTimer.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
		float timeRemaining = matchSoftReset.PauseTimeRemaining;
        bool visible = timeRemaining > 0f;

		labelTimer.text = (1 + (int)timeRemaining).ToString();
		labelTimer.gameObject.SetActive (visible);
    }
}
