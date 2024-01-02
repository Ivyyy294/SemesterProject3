using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

//[RequireComponent (typeof (PlayerBallStatus))]
public class PlayerOxygen : NetworkBehaviour
{
	[Range (1f, 1000f)]
	[SerializeField] float maxOxygen = 100f;

	[Range (1f, 100f)]
	[SerializeField] float passiveOxygenConsumption = 10f;

	//Public
	public bool OxygenEmpty { get {return currentOxygen <= 0f;} }

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (currentOxygen));
	}

	//Private
	//PlayerBallStatus playerBallStatus;
	float currentOxygen;

	// Start is called before the first frame update
	void Start()
	{
		currentOxygen = maxOxygen;
		//playerBallStatus = GetComponent<PlayerBallStatus>();
	}

	// Update is called once per frame
	void Update()
    {
		if (!Owner && networkPackage.Available)
		{
			currentOxygen = networkPackage.Value(0).GetFloat();
		}
		else
		{
			if (Owner)
				Debug.Log ("Oxygen: " + currentOxygen);

			if (currentOxygen > 0f)
				currentOxygen -= passiveOxygenConsumption * Time.deltaTime;
		}
    }
}
