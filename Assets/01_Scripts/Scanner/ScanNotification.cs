using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ScannableObject), typeof (GeneticInformation))]
public class ScanNotification : MonoBehaviour
{
	[SerializeField] AudioAsset audioAsset;
	ScannableObject scannableObject;
	GeneticInformation geneticInformation;
	bool done;

    // Start is called before the first frame update
    void Start()
    {
        done = false;
		scannableObject = GetComponent <ScannableObject>();
		geneticInformation = GetComponent <GeneticInformation>();
    }

    // Update is called once per frame
    void Update()
    {
		if (!done && scannableObject.IsScanned)
		{
			audioAsset.PlayOneShot();
			Debug.Log ("Scanned: " + geneticInformation.NameSpecies);
			done = true;
		}
    }
}
