using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using System;

struct ColliderData
{
	public Rigidbody rigidbody;
	public Vector3 direction;
	public float distanz;
}

public class CurrentController : MonoBehaviour
{
	[Header("Shape Settings")]
	[SerializeField] List <Transform> pointList = new List<Transform>();
	[SerializeField] float range;

	[Header("Force Settings")]
	[Range (0f, 500f)]
	[SerializeField] float forcePlayer;
	[Range (0f, 500f)]
	[SerializeField] float forceBall;
	[Range (0f, 500f)]
	[SerializeField] float forceOxygen;

	[HideInInspector]
	[SerializeField] Mesh gizmoMesh;

	private void FixedUpdate()
	{
		List <ColliderData> affectedColliderDataList = GetAffectedColliderData();
		ApplyForce (affectedColliderDataList);
	}
	
	private List <ColliderData> GetAffectedColliderData ()
	{
		List <ColliderData> affectedColliderDataList = new List <ColliderData>();

		for (int i = 1; i < pointList.Count; ++i)
		{
			Vector3 start = pointList[i-1].position;
			Vector3 end = pointList[i].position;
			//Vector3 direction = (end - start).normalized;
			var colliderList = Physics.OverlapCapsule (start, end, range, Physics.AllLayers, QueryTriggerInteraction.Ignore);

			foreach (Collider collider in colliderList)
			{
				Rigidbody _rigidbody = collider.GetComponentInParent<Rigidbody>();

				//Only add colliders with a Rigidbody to list
				if (_rigidbody != null)
				{
					ColliderData pendingColliderData;
					pendingColliderData.rigidbody = _rigidbody;
					pendingColliderData.distanz = Vector3.Distance (end, collider.transform.position);

					Vector3 forwardBase = (end - start).normalized;
					Vector3 forwardObj = (end - _rigidbody.position).normalized;
					float angle = Vector3.Angle(forwardBase, forwardObj);

					//Prevents objects from getting trapped near end
					if (angle >= 90f)
						pendingColliderData.direction = (end-start).normalized;
					else
						pendingColliderData.direction = (end - _rigidbody.position).normalized;
					
					bool included = false;
					//Check if the rigidbody is already in list
					for (int j = 0; j < affectedColliderDataList.Count; ++j)
					{
						ColliderData tmp = affectedColliderDataList[j];

						//Override existing data with pending if distanz is smaller
						if (tmp.rigidbody == pendingColliderData.rigidbody)
						{
							if (tmp.distanz < pendingColliderData.distanz)
								affectedColliderDataList[j] = pendingColliderData;
							
							included = true;
							break;
						}
					}

					if (!included)
						affectedColliderDataList.Add (pendingColliderData);
				}
			}
		}

		return affectedColliderDataList;
	}

	private void ApplyForce (List <ColliderData> colliderDataList)
	{
		foreach (ColliderData colliderData in colliderDataList)
		{
			Rigidbody _rigidbody = colliderData.rigidbody;
			AffectedByCurrent affectedByCurrent = _rigidbody.GetComponentInChildren<AffectedByCurrent>();

			if (affectedByCurrent != null)
			{
				float forceDelta = GetForce(_rigidbody) * Time.fixedDeltaTime;
				_rigidbody.AddForce (colliderData.direction * forceDelta, ForceMode.Acceleration);
				//_rigidbody.transform.forward = Vector3.MoveTowards (_rigidbody.transform.forward, colliderData.direction, forceDelta);
			}
		}
	}

	private float GetForce (Rigidbody rb)
	{
		AffectedByCurrent affectedByCurrent = rb.GetComponentInChildren<AffectedByCurrent>();

		if (affectedByCurrent)
		{
			switch (affectedByCurrent.GetForceTyp)
			{
				case AffectedByCurrent.ForceTyp.PLAYER:
					return forcePlayer;
				case AffectedByCurrent.ForceTyp.BALL:
					return forceBall;
				case AffectedByCurrent.ForceTyp.OXYGEN:
					return forceOxygen;
			}

		}

		return 0f;
	}

	private void OnDrawGizmos()
	{
		float diameter = range * 2f;

		for (int i = 0; i < pointList.Count; ++i)
		{
			Transform point = pointList[i];

			//Draw Point Gizmo
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (point.position, 0.1f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (point.position, range);

			//Draw Cylinder
			if (i > 0)
			{
				Transform previousPoint = pointList[i - 1];
				Vector3 posDiff = point.position - previousPoint.position;
				float dist = Vector3.Distance (point.position, previousPoint.position);
				Quaternion rotation = Quaternion.LookRotation (previousPoint.position - point.position, Vector3.up);
				rotation *= Quaternion.Euler (Vector3.right * 90f);

				//Quaternion rotation = Quaternion.Euler (90f, 0f, 0f);
				Gizmos.DrawWireMesh (gizmoMesh, previousPoint.position + posDiff * 0.5f, rotation, new Vector3 (diameter, dist * 0.5f, diameter));
			}
		}
	}
}
