using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenSpawner : MonoBehaviour
{
	[Range (1f, 120f)]
	[SerializeField] float spawnIntervall;

	[Min (1f)]
	[SerializeField] float spawnOxygen;

	[Header ("Lara Values")]
	[SerializeField] GameObject oxygenBubble;
	[SerializeField] GameObject spawnPos;

	float internTimer;
	OxygenMovement oxygenMovement;
	OxygenRefill oxygenRefill;

	private void Start()
	{
		oxygenBubble.SetActive (false);
		oxygenMovement = oxygenBubble.GetComponent<OxygenMovement>();
		oxygenRefill = oxygenBubble.GetComponent<OxygenRefill>();

		//Only enable when local instance is owner of oxygenMovement
		enabled = oxygenMovement && oxygenRefill && spawnPos && (!NetworkManager.Me || NetworkManager.Me.Host);
	}

	// Update is called once per frame
	void Update()
    {
        if (!oxygenMovement.gameObject.activeInHierarchy)
		{
			if (internTimer <= spawnIntervall)
				internTimer += Time.deltaTime;
			else
			{
				internTimer = 0f;
				oxygenMovement.SpawnAt (spawnPos.transform.position);
				oxygenRefill.SetCurrentOxygen (spawnOxygen);
			}
		}
    }
}
