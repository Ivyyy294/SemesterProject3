using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] CinemachineVirtualCamera virtualCameraDefault;

	[Header ("Up Camera")]
	[Range (0, 1)]
	[SerializeField] float angleThresholdUp;
	[SerializeField] CinemachineVirtualCamera virtualCameraUp;

	[Header ("Down Camera")]
	[Range (0, -1)]
	[SerializeField] float angleThresholdDown;
	[SerializeField] CinemachineVirtualCamera virtualCameraDown;

	CinemachineVirtualCamera activeVirtualCamera = null;
	CameraTarget cameraTarget;

	//Public Methods

	public Camera MainCamera => mainCamera;
	public static CameraSystem Me {get; private set;}

	public void InitCameraTarget (CameraTarget target)
	{
		cameraTarget = target;

		if (target != null)
		{
			SetCameraTarget(virtualCameraDefault, cameraTarget.transform);
			SetCameraTarget(virtualCameraUp, cameraTarget.transform);
			SetCameraTarget(virtualCameraDown, cameraTarget.transform);
		}
	}

	public void OnTargetObjectWarped (Transform target, Vector3 posOffset)
	{
		if (activeVirtualCamera)
			activeVirtualCamera.OnTargetObjectWarped (target, posOffset);
	}

	public void EnableVCam (bool val)
	{
		if (activeVirtualCamera)
			activeVirtualCamera.gameObject.SetActive (val);
	}

	//Private Methods

	private void Update()
	{
		if (cameraTarget != null)
		{
			CinemachineVirtualCamera targetCamera = virtualCameraDefault;

			float currentAngle = cameraTarget.CurrentY;

			if (currentAngle >= angleThresholdUp)
				targetCamera = virtualCameraUp;
			else if (currentAngle <= angleThresholdDown)
				targetCamera = virtualCameraDown;

			if (targetCamera != activeVirtualCamera)
				SwitchCamera (targetCamera);
		}
	}

	private void OnEnable()
    {
		if (Me == null)
		{
			Me = this;
			mainCamera.gameObject.SetActive (false);
			//virtualCameraDefault.gameObject.SetActive (true);
			//activeVirtualCamera = virtualCameraDefault;
			//activeVirtualCamera.gameObject.SetActive (false);
		}
		else
			Destroy (this);
    }

	void OnDestroy()
	{
		Me = null;
	}

	private void SetCameraTarget (CinemachineVirtualCamera vcam, Transform target)
	{
		if (target != null)
		{
			vcam.Follow = target;
			vcam.LookAt = target;

			Vector3 posOffset = vcam.transform.position - target.position;
			vcam.OnTargetObjectWarped (target, posOffset);
			
			mainCamera.gameObject.SetActive (true);
		}
	}

	private void SwitchCamera (CinemachineVirtualCamera newVcam)
	{
		newVcam.gameObject.SetActive (true);

		if (activeVirtualCamera != null)
			activeVirtualCamera.gameObject.SetActive(false);
		
		activeVirtualCamera = newVcam;
	}
}
