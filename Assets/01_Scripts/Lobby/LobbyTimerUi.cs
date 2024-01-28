using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyTimerUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI labelTimer;
	[SerializeField] Lobby lobby;

    // Start is called before the first frame update
    void Start()
    {
		labelTimer.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
		float timeRemaining = lobby.TimeUntilStart;
        bool visible = lobby.AllPayersReady();

		labelTimer.text = (1 + (int)timeRemaining).ToString();
		labelTimer.gameObject.SetActive (visible);
    }
}
