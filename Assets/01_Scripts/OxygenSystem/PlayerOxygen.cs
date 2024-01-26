using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

//[RequireComponent (typeof (PlayerBallStatus))]
public class PlayerOxygen : NetworkBehaviour
{
	[Range (1f, 1000f)]
	[SerializeField] float maxOxygen = 100f;

	[Range (0f, 100f)]
	[SerializeField] float passiveOxygenConsumption = 10f;

	[Range (0f, 100f)]
	[SerializeField] float dashOxygenConsumption = 10f;

	float currentOxygen;
	float timer;
	PlayerInput playerInput;

	//Public
	public bool OxygenEmpty { get {return currentOxygen <= 0f;} }
	public float CurrentOxygen => currentOxygen;
	public float CurrentOxygenPercent {get{return (currentOxygen / maxOxygen) * 100;} }

	public void Refill (float val)
	{
		if (currentOxygen < maxOxygen)
			currentOxygen = Mathf.Min (currentOxygen + val, maxOxygen);
	}
	
	public void RefillMax()
	{
		currentOxygen = maxOxygen;
	}

	//Protected
	protected override void SetPackageData()
	{
		networkPackage.AddValue (new NetworkPackageValue (currentOxygen));
	}

	//Private
	// Start is called before the first frame update
	void Start()
	{
		currentOxygen = maxOxygen;
		playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
	}

	// Update is called once per frame
	void Update()
    {
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;

		if (!Owner && networkPackage.Available)
			currentOxygen = networkPackage.Value(0).GetFloat();

		if (currentOxygen > 0f)
			currentOxygen -= passiveOxygenConsumption * Time.deltaTime;

		if (playerInput.DashPressed)
			currentOxygen -= dashOxygenConsumption * Time.deltaTime;
    }
}
