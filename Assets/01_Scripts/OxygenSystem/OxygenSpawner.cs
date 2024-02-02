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

	OxygenBubbleNetwork oxygenBubbleNetwork;
	OxygenBubbleRefill oxygenRefill;

	private void Start()
	{
		oxygenBubbleNetwork = GetComponent<OxygenBubbleNetwork>();
		oxygenRefill = oxygenBubble.GetComponent<OxygenBubbleRefill>();

		GetRandomSpawnTime();
	}

	// Update is called once per frame
	void Update()
    {
		if (spawnTime == -1f)
			GetRandomSpawnTime();

		if (!oxygenBubble.gameObject.activeInHierarchy)
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

		oxygenRefill.SetCurrentOxygen (spawnOxygen);
		oxygenBubbleNetwork.SpawnAt (spawnPos.transform.position);
	}

	void GetRandomSpawnTime()
	{
		spawnTime = Random.Range (minSpawnTime, maxSpawnTime);
	}
	
}
