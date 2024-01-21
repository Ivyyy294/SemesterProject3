using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHostSession : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject sessionContainer;
	[SerializeField] GameObject hostSessionEntryPrefab;
	public Button backButton;

    NetworkManagerHostSessionExplorer searchHostSession = new NetworkManagerHostSessionExplorer();
	List <HostSessionData> hostSessionList = new List<HostSessionData>();

	List <GameObject> sessionList = new List<GameObject>();

	private void OnEnable()
	{
		hostSessionList.Clear();
		searchHostSession.StartSearchHostSession();
	}

	private void OnDisable()
	{
		searchHostSession.ShutDownSearchHostSession();

		foreach (GameObject i in sessionList)
			Destroy (i);

		sessionList.Clear();
	}

    // Update is called once per frame
    void Update()
    {
        if (hostSessionList.Count < searchHostSession.HostSessionList.Count)
			AddNewHostSession();
    }

	void AddNewHostSession()
	{
		List <HostSessionData> newList = searchHostSession.HostSessionList;

		foreach (var i in newList)
		{
			if (!hostSessionList.Contains (i))
			{
				hostSessionList.Add (i);
				HostSessionEntry newHostSessionEntry = Instantiate (hostSessionEntryPrefab, sessionContainer.transform).GetComponent<HostSessionEntry>();
				newHostSessionEntry.lobbyName.text = i.lobbyName;
				newHostSessionEntry.joinButton.onClick.AddListener(()=> {JoinHostSession (i.ip);});
				sessionList.Add (newHostSessionEntry.gameObject);
			}
		}
	}

	void JoinHostSession (string ip)
	{
		networkManagerCallback.OnClientStarted (ip);
	}
}
