using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System;

public class PassageController : NetworkBehaviour
{
	[SerializeField] GameObject[] passageList;

	[Min (1)]
	[SerializeField] int openPassagesCount = 1;
	int[] openPassages;
	bool initDone = false;

	//Protected
	protected override void SetPackageData()
	{
		for (int i = 0; i < openPassagesCount; ++i)
			networkPackage.AddValue (new NetworkPackageValue (openPassages[i]));
	}

    // Start is called before the first frame update
    void Start()
    {
        openPassages = new int[openPassagesCount];

		Owner = !NetworkManager.Me || NetworkManager.Me.Host;

		if (openPassagesCount >= passageList.Length)
		{
			Debug.LogError ("openPassagesCount can not be greater as " + passageList.Length);
			gameObject.SetActive (false);
			return;
		}

		if (Owner)
		{
			for (int i = 0; i < openPassagesCount; ++i)
				openPassages[i] = GetRandomPassage();
		}
    }

    // Update is called once per frame
    void Update()
    {
		//Return if passages are already open
		if (initDone)
			return;
		else if (Owner)
			InitPassages();
        else if (networkPackage.Count == openPassagesCount)
		{
			for (int i = 0; i < openPassagesCount; ++i)
				openPassages[i] = networkPackage.Value(i).GetInt32();

			InitPassages();
		}
    }

	int GetRandomPassage()
	{
		int passageIndex = UnityEngine.Random.Range (0, passageList.Length -1);

		while (Array.IndexOf(openPassages, passageIndex) > -1)
			passageIndex = UnityEngine.Random.Range (0, passageList.Length -1);

		return passageIndex;
	}

	void InitPassages()
	{
		for (int i = 0; i < openPassagesCount; ++i)
		{
			int index = openPassages[i];

			if (index < passageList.Length)
				passageList[index].SetActive(false);
			else
				Debug.LogError ("openPassagesCount can not be greater as " + passageList.Length);
		}

		initDone = true;
	}
}
