using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHostSession : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
	[SerializeField] GameObject sessionContainer;
	[SerializeField] GameObject hostSessionEntryPrefab;

    NetworkManagerHostSessionExplorer searchHostSession = new NetworkManagerHostSessionExplorer();
	List <HostSessionData> hostSessionList = new List<HostSessionData>();

	private void OnEnable()
	{
		searchHostSession.StartSearchHostSession();
	}

	private void OnDisable()
	{
		searchHostSession.ShutDownSearchHostSession();
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
			}
		}
	}

	void JoinHostSession (string ip)
	{
		networkManagerCallback.OnClientStarted (ip);
	}
}
