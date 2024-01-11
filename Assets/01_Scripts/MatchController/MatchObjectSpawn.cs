using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchObjectSpawn : MonoBehaviour
{
	[Header ("Object References")]
	[SerializeField] GameObject ball;
	[SerializeField] PlayerManager playerManager;

	[Header ("Spawn Points")]
	[SerializeField] Transform ballSpawnPoint;
	[SerializeField] Transform[] team1SpawnPoint;
	[SerializeField] Transform[] team2SpawnPoint;

	bool initialSpawnDone = false;

	public void RespawnObjects()
	{
		if (ball && ballSpawnPoint)
			RespawnObject (ball, ballSpawnPoint);

		if (playerManager && team1SpawnPoint != null && team2SpawnPoint != null)
		{
			//Local Debug
			if (PlayerConfigurationManager.Me == null)
				RespawnObject (playerManager.PlayerList[0], team1SpawnPoint[0]);
			else
			{
				int indexTeam1 = 0;
				int indexTeam2 = 0;

				foreach (GameObject i in playerManager.PlayerList)
				{
					PlayerConfiguration playerConfiguration = i.GetComponentInChildren<PlayerConfigurationContainer>().playerConfiguration;

					if (playerConfiguration.connected)
					{
						int playerTeamIndex = playerConfiguration.teamNr;

						if (playerTeamIndex == 0)
							RespawnObject (i, team1SpawnPoint[indexTeam1++]);
						else
							RespawnObject (i, team2SpawnPoint[indexTeam2++]);
					}
				}
			}
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        if (!ball)
			Debug.LogError("Missing Ball Reference!");

		if (!playerManager)
			Debug.LogError("Missing PlayerManager Reference!");

		if (!ballSpawnPoint)
			Debug.LogError("Missing ballSpawnPoint Reference!");

		if (team1SpawnPoint == null)
			Debug.LogError("Missing team1SpawnPoint Reference!");

		if (team2SpawnPoint == null)
			Debug.LogError("Missing team2SpawnPoint Reference!");
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialSpawnDone)
		{
			RespawnObjects();
			initialSpawnDone = true;
		}
    }

	void RespawnObject (GameObject obj, Transform target)
	{
		obj.transform.position = target.transform.position;
		obj.transform.forward = target.transform.forward;
	}
}
