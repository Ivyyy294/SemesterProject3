using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System.Linq;

struct ScanInfoData
{
	public float scanTime;
	public GeneticInformationAsset geneticInformation;

	public bool IsScanned()
	{
		return scanTime >= geneticInformation.ScanTime;
	}
}

public class ScannableObjectManager : NetworkBehaviour
{
	static public ScannableObjectManager me;
	
	[SerializeField] Scanner[] playerScanner;
	[SerializeField] AudioAsset audioScanSuccesful;

	private Dictionary <string, ScanInfoData> geneticInformationMap = new Dictionary<string, ScanInfoData>();
	private List <string> scannedObjects = new List<string>();

	//Public
	public bool IsScanned (string guid)
	{
		return scannedObjects.Contains (guid);
	}

	//Protected
	protected override void SetPackageData()
	{
		//expensive......
		foreach (string i in scannedObjects)
			networkPackage.AddValue (new NetworkPackageValue (i));
	}

	//Private
	private void Start()
	{
		InitScannableObjectMap();
		me = this;
		Owner = !NetworkManager.Me || NetworkManager.Me.Host;
	}

	private void Update()
	{
		if (Owner)
			HostUpdate();
		else
			ClientUpdate();
	}

	private void InitScannableObjectMap()
	{
		foreach (var i in GameObject.FindObjectsOfType <ScannableObject>())
		{
			GeneticInformationAsset geneticInformation = i.GeneticInformation;

			if (!geneticInformationMap.ContainsKey (geneticInformation.GUID))
			{
				ScanInfoData scanInfoData;
				scanInfoData.scanTime = 0f;
				scanInfoData.geneticInformation = geneticInformation;
				geneticInformationMap.Add (geneticInformation.GUID, scanInfoData);
			}
		}
	}

	private void HostUpdate()
	{
		List <string> targetList = new List<string>();

		foreach (Scanner i in playerScanner)
		{
			if (i.CurrentTargetId.Length > 0)
				targetList.Add (i.CurrentTargetId);
		}

		foreach (string i in targetList)
		{
			ScanInfoData scanInfoData = geneticInformationMap[i];
			string guid = scanInfoData.geneticInformation.GUID;

			if (!IsScanned (guid))
			{
				if (scanInfoData.geneticInformation.DualScanRequired
					&& targetList.Count(item => item == i) > 1
					|| !scanInfoData.geneticInformation.DualScanRequired)
				{
					scanInfoData.scanTime += Time.deltaTime;
					geneticInformationMap[i] = scanInfoData;
					Debug.Log ("Scanning: " + scanInfoData.geneticInformation.name);

					if (scanInfoData.IsScanned() && audioScanSuccesful)
					{
						scannedObjects.Add (guid);
						audioScanSuccesful.PlayOneShot();
						Debug.Log ("Scan complete: " + scanInfoData.geneticInformation.name + "!");
					}
				}
			}
		}
	}
	private void ClientUpdate()
	{
		if (networkPackage.Available)
		{
			audioScanSuccesful.PlayOneShot();
			
			while (networkPackage.Count > scannedObjects.Count)
			{
				string guid = networkPackage.Value(scannedObjects.Count).GetString();
				scannedObjects.Add (guid);
				Debug.Log ("Scan complete: " + guid + "!");
			}
		}
	}
}
