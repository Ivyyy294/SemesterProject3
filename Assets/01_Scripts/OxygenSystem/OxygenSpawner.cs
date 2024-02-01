using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenSpawner : MonoBehaviour
{
	[Range (0f, 120f)]
	[SerializeField] float minSpawnTime = 1f;
	[Range (1f, 120f)]
	[SerializeField] float maxSpawnTime = 10f;

	[Min (1f)]
	[SerializeField] float spawnOxygen;

	[Header ("Lara Values")]
	[SerializeField] GameObject oxygenBubble;
	[SerializeField] GameObject spawnPos;

	float internTimer;
	float spawnTime = -1;

	OxygenBubbleMovement oxygenMovement;
	OxygenBubbleRefill oxygenRefill;

	private void Start()
	{
		oxygenBubble.SetActive (false);
		oxygenMovement = oxygenBubble.GetComponent<OxygenBubbleMovement>();
		oxygenRefill = oxygenBubble.GetComponent<OxygenBubbleRefill>();

		//Only enable when local instance is owner of oxygenMovement
		enabled = oxygenMovement && oxygenRefill && spawnPos && (!NetworkManager.Me || NetworkManager.Me.Host);

		GetRandomSpawnTime();
	}

	// Update is called once per frame
	void Update()
    {
		if (spawnTime == -1f)
			GetRandomSpawnTime();

		if (!oxygenMovement.gameObject.activeInHierarchy)
		{
			if (internTimer <= spawnTime)
				internTimer += Time.deltaTime;
			else
				SpawnOxygenBubble();
		}
    }

	void SpawnOxygenBubble()
	{
		internTimer = 0f;
		spawnTime = -1f;

		oxygenMovement.transform.position = spawnPos.transform.position;
		oxygenMovement.gameObject.SetActive (true);
		oxygenRefill.SetCurrentOxygen (spawnOxygen);
	}

	void GetRandomSpawnTime()
	{
		spawnTime = Random.Range (minSpawnTime, maxSpawnTime);
	}
	
}
