using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public float CurrentY {get; private set; }

	private void Start()
	{
		bool Owner = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().IsLocalPlayer();

		if (!Owner)
			enabled = false;
		else
			CameraSystem.Me.InitCameraTarget (this);
	}

	// Update is called once per frame
	void Update()
    {
        var lookVector = Vector3.Cross(transform.parent.right, Vector3.up);
		CurrentY = transform.parent.forward.y;
        lookVector.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
    }
}
