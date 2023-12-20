using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannableObject : MonoBehaviour
{
	[SerializeField] GeneticInformationAsset geneticInformation;

	//Public
	public GeneticInformationAsset GeneticInformation { get { return geneticInformation;} }

	public bool IsScanned ()
	{
		return	ScannableObjectManager.me.IsScanned (geneticInformation.GUID);
	}
}
