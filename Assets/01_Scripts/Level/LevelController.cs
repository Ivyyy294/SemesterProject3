using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	PlayerConfigurationManager configurationManager;
	int indexLocalPlayer;
	bool waiting = true;

    // Start is called before the first frame update
    void Start()
    {
		//Freez time until all players are ready
        configurationManager = PlayerConfigurationManager.Me;

		//Report that this instance finished loading
		if (configurationManager)
		{
			Time.timeScale = 0f;
			indexLocalPlayer = configurationManager.LocalPlayerId;
			configurationManager.playerConfigurations[indexLocalPlayer].sceneLoaded = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (configurationManager)
		{
			//Wait for all instances to finish loading
			if (waiting && configurationManager.PlayersLoadedScene())
			{
				waiting = false;
				Time.timeScale = 1f;
			}
		}
    }
}
