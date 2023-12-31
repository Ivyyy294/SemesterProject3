using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField] private Transform cameraPivot;

    // Update is called once per frame
    void Update()
    {
        UpdateCameraTransforms();
    }

	void UpdateCameraTransforms()
    {
        var lookVector = Vector3.Cross(transform.right, Vector3.up);
        lookVector.y = 0;
        cameraPivot.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
    }
}
