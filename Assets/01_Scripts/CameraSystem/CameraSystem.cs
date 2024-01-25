using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] CinemachineVirtualCamera virtualCamera;

	public Camera MainCamera => mainCamera;
	public static CameraSystem Me {get; private set;}

	public void SetCameraTarget (Transform target)
	{
		if (target != null)
		{
			virtualCamera.Follow = target;
			virtualCamera.LookAt = target;
			virtualCamera.gameObject.SetActive (true);

			Vector3 posOffset = virtualCamera.transform.position - target.position;
			virtualCamera.OnTargetObjectWarped (target, posOffset);
			
			mainCamera.gameObject.SetActive (true);
		}
	}

	public void OnTargetObjectWarped (Transform target, Vector3 posOffset)
	{
		virtualCamera.OnTargetObjectWarped (target, posOffset);
	}

	public void EnableVCam (bool val)
	{
		virtualCamera.gameObject.SetActive (val);
	}

	private void OnEnable()
    {
		if (Me == null)
		{
			Me = this;
			mainCamera.gameObject.SetActive (false);
			virtualCamera.gameObject.SetActive (false);
		}
		else
			Destroy (this);
    }

	void OnDestroy()
	{
		Me = null;
	}
}
