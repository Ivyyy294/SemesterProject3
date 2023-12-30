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

		if (networkManager)
		{
			for (int i = 0; i < playerList.Length; ++i)
			{
				SetOwnerState (playerList[i], playerConfigurationManager.playerConfigurations[i].Owner);
				playerList[i].SetActive (playerConfigurationManager.playerConfigurations[i].connected);
			}
		}
		else
		{
			for (int i = 0; i < playerList.Length; ++i)
			{
				SetOwnerState (playerList[i], i == 0);
				playerList[i].SetActive (i == 0);
			}
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
