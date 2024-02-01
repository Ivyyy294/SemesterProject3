using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class OxygenBubbleNetwork : NetworkBehaviour
{
	OxygenBubbleRefill oxygenRefill;
	OxygenBubbleMovement oxygenBubbleMovement;

	[RPCAttribute]
	public void Spawn()
	{
		if (!Owner)
			gameObject.SetActive(true);

		oxygenBubbleMovement.Spawn();
	}

	[RPCAttribute]
	public void Despawn()
	{
		if (!Owner)
			gameObject.SetActive(false);
	}

    // Start is called before the first frame update
    void Start()
    {
		if (!NetworkManager.Me)
		{
			enabled = false;
			return;
		}

        oxygenRefill = GetComponent<OxygenBubbleRefill>();
		oxygenBubbleMovement = GetComponent <OxygenBubbleMovement>();
		Owner = NetworkManager.Me.Host;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Owner && networkPackage.Available)
		{
			transform.position = networkPackage.Value(0).GetVector3();
			oxygenRefill.SetCurrentOxygen (networkPackage.Value(1).GetFloat());
			networkPackage.Clear();
		}
    }

	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (transform.position));
		networkPackage.AddValue (new NetworkPackageValue (oxygenRefill.CurrentOxygen));
	}

	private void OnEnable()
	{
		if (Owner)
		{
			Spawn();
			InvokeRPC("Spawn");
		}
	}

	private void OnDisable()
	{
		if (Owner)
			InvokeRPC("Despawn");
	}
}
