using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewGeneticInformation", menuName = "GeneticInformation")]
public class GeneticInformationAsset : ScriptableObject
{
	[Header ("Scanner Settings")]
	[HideInInspector][SerializeField] string guid = System.Guid.NewGuid().ToString();
	[Min (0.1f)][SerializeField] float scanTime;
	[Min (0.1f)][SerializeField] float scanRange;
	[SerializeField] bool dualScanRequired = false;

	[Header ("Genetic Information")]
	[SerializeField] string nameSpecies;

	//Public
	public string GUID { get { return guid;} }
	public string NameSpecies { get { return nameSpecies;} }
	public float ScanTime { get { return scanTime;} }
	public float ScanRange { get { return scanRange;} }
	public bool DualScanRequired { get { return dualScanRequired;} }
}
