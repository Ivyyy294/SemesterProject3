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

public class ScannableObjectManager : MonoBehaviour
{
	static public ScannableObjectManager me;
	
	[SerializeField] Scanner[] playerScanner;
	[SerializeField] AudioAsset audioScanSuccesful;

	private Dictionary <string, ScanInfoData> geneticInformationMap = new Dictionary<string, ScanInfoData>();

	//Public
	public bool IsScanned (string guid)
	{
		if (geneticInformationMap.ContainsKey (guid))
			return geneticInformationMap[guid].IsScanned();

		return false;
	}

	//Private
	private void Start()
	{
		InitScannableObjectMap();
		me = this;
		//Owner = !NetworkManager.Me || NetworkManager.Me.Host;
	}

	private void Update()
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

			if (!scanInfoData.IsScanned())
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
						audioScanSuccesful.PlayOneShot();
						Debug.Log ("Scan complete: " + scanInfoData.geneticInformation.name + "!");
					}
				}
			}
		}
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
}
