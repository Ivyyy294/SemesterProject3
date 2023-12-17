using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] GameObject[] playerList = new GameObject[2];
	PlayerConfigurationManager playerConfigurationManager;
	NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        playerConfigurationManager = PlayerConfigurationManager.Me;
		networkManager = NetworkManager.Me;

		if (playerConfigurationManager && networkManager)
		{
			SetOwnerState (playerList[0], networkManager.Host);
			SetOwnerState (playerList[1], !networkManager.Host);
		}
		//Player one is default
		else
		{
			SetOwnerState (playerList[0], true);

			//Disable player 2
			playerList[1].SetActive (false);
		}
    }

	private void SetOwnerState (GameObject obj, bool val)
	{
		if (obj)
		{
			NetworkBehaviour[] networkBehaviours = obj.GetComponentsInChildren <NetworkBehaviour>();

			foreach (NetworkBehaviour i in networkBehaviours)
				i.Owner = val;

			//Disable camera on remote player
			if (!val)
			{
				Camera camera = obj.GetComponentInChildren<Camera>();
				camera.gameObject.SetActive (false);
			}
		}
	}
}
