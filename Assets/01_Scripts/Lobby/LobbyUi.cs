using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI labelTimer;
	[SerializeField] TextMeshProUGUI labelWaiting;
	[SerializeField] TextMeshProUGUI labelUnevenTeams;
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

		bool playersReady = lobby.AllPayersReady();
		bool equalTeamSize = lobby.EqualTeamSize();

		labelWaiting.gameObject.SetActive (!playersReady);
		labelUnevenTeams.gameObject.SetActive (playersReady && !equalTeamSize);


        bool visible = playersReady && equalTeamSize;

		labelTimer.text = (1 + (int)timeRemaining).ToString();
		labelTimer.gameObject.SetActive (visible);
    }
}
