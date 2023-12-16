using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
	[Min(0.1f)]
	[SerializeField] float range = 0f;

	[Min(0.1f)]
	[SerializeField] float diameter = 0f;

	[Header ("Lara Values")]
	[SerializeField] GameObject scannerCollider;

	bool active = false;

	public void SetActivate (bool val)
	{
		active = val;
	}

    // Start is called before the first frame update
    void Start()
    {
		ScaleScannerCollider();
    }

	private void Update()
	{
		if (scannerCollider.activeInHierarchy != active)
			scannerCollider.SetActive (active);
	}

	void ScaleScannerCollider()
	{
        if (scannerCollider)
		{
			scannerCollider.transform.localScale = new Vector3 (diameter, range, diameter);
			scannerCollider.transform.localPosition = new Vector3 (0, 0, range);
		}
	}
}

