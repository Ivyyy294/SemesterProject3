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

	public void RespawnObjects()
	{
		if (ball && ballSpawnPoint)
		{
			RespawnObject (ball, ballSpawnPoint);
			ball.SetActive (true);
		}

		if (playerManager && team1SpawnPoint != null && team2SpawnPoint != null)
		{
			//Local Debug
			if (PlayerConfigurationManager.Me == null)
			{
				CameraSystem.Me.EnableVCam (false);
				RespawnObject (playerManager.PlayerList[0], team1SpawnPoint[0]);
				Vector3 posOffset = team1SpawnPoint[0].position - playerManager.PlayerList[0].transform.position;
				CameraSystem.Me.OnTargetObjectWarped (playerManager.PlayerList[0].transform, posOffset);
				CameraSystem.Me.EnableVCam (true);
			}
			else
			{
				int indexTeam1 = 0;
				int indexTeam2 = 0;

				foreach (GameObject i in playerManager.PlayerList)
				{
					PlayerConfigurationContainer playerConfigurationContainer = i.GetComponentInChildren<PlayerConfigurationContainer>();
					PlayerConfiguration playerConfiguration = playerConfigurationContainer.playerConfiguration;

					if (playerConfiguration.connected)
					{
						if (playerConfigurationContainer.IsLocalPlayer())
							CameraSystem.Me.EnableVCam (false);

						int playerTeamIndex = playerConfiguration.teamNr;

						if (playerTeamIndex == 0)
							RespawnObject (i, team1SpawnPoint[indexTeam1++]);
						else
							RespawnObject (i, team2SpawnPoint[indexTeam2++]);

						if (playerConfigurationContainer.IsLocalPlayer())
							CameraSystem.Me.EnableVCam (true);
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

	void RespawnObject (GameObject obj, Transform target)
	{
		obj.transform.position = target.transform.position;
		obj.transform.forward = target.transform.forward;

		//Null Velocity
		Rigidbody rb = obj.GetComponentInChildren<Rigidbody>();

		if (rb != null)
			rb.velocity = Vector3.zero;

		//Null Player RefSpeed
		PlayerMovement playerMovement = obj.GetComponentInChildren<PlayerMovement>();

		if (playerMovement != null)
			playerMovement.ResetRefSpeed();
	}
}
