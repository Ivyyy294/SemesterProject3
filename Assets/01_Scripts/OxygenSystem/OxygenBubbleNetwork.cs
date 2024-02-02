using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenBubbleNetwork : NetworkBehaviour
{
	[SerializeField] GameObject bubble;

	OxygenBubbleRefill oxygenRefill;
	OxygenBubbleMovement oxygenBubbleMovement;

	public OxygenBubbleRefill OxygenRefill => oxygenRefill;
	public OxygenBubbleMovement OxygenBubbleMovement => oxygenBubbleMovement;

	public void SpawnAt (Vector3 pos)
	{
		bubble.SetActive(true);
		bubble.transform.position = pos;
		oxygenBubbleMovement.Spawn();
	}

	public void DespawnBubble()
	{
		Debug.Log ("DespawnBubble");
		bubble.SetActive(false);
	}

    void Awake()
    {
        oxygenRefill = bubble.GetComponent<OxygenBubbleRefill>();
		oxygenBubbleMovement = bubble.GetComponent <OxygenBubbleMovement>();

		if (!NetworkManager.Me)
		{
			enabled = false;
			return;
		}

		Owner = NetworkManager.Me.Host;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Owner && networkPackage.Available)
		{
			if (OxygenBubbleMovement.Rigidbody)
				OxygenBubbleMovement.Rigidbody.isKinematic = true;

			//Set Position
			bubble.transform.position = networkPackage.Value(0).GetVector3();

			//Set Oxygen
			oxygenRefill.SetCurrentOxygen (networkPackage.Value(1).GetFloat());
			
			//Set active
			bool active = networkPackage.Value(2).GetBool();

			if (active && !bubble.activeInHierarchy)
				SpawnAt(transform.position);
			else if (!active && bubble.activeInHierarchy)
				DespawnBubble();

			networkPackage.Clear();
		}
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (bubble.transform.position));
		networkPackage.AddValue (new NetworkPackageValue (oxygenRefill.CurrentOxygen));
		networkPackage.AddValue (new NetworkPackageValue (oxygenBubbleMovement.gameObject.activeInHierarchy));
	}
}
