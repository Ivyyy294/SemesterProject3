using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSettings : MonoBehaviour
{
	[SerializeField] KeyCode keySettings;
	[SerializeField] GameObject settingsObj;

	private void Start()
	{
		settingsObj.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(keySettings))
			settingsObj.SetActive (!settingsObj.activeInHierarchy);
    }
}
