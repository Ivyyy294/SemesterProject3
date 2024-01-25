using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	private void Start()
	{
		bool Owner = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().IsLocalPlayer();

		if (!Owner)
			enabled = false;
		else
			CameraSystem.Me.SetCameraTarget (transform);
	}

	// Update is called once per frame
	void Update()
    {
        var lookVector = Vector3.Cross(transform.parent.right, Vector3.up);
        lookVector.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
    }
}
