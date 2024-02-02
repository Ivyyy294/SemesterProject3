using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class AudioTriggerAmbientDeep : MonoBehaviour
{
	[SerializeField] GameEvent eventAmbientNormal;
	[SerializeField] GameEvent eventAmbientDeep;

	private void OnTriggerEnter(Collider other)
	{
		if (IsLocalPlayer(other))
			eventAmbientDeep?.Raise();
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsLocalPlayer(other))
			eventAmbientNormal?.Raise();
	}

	bool IsLocalPlayer (Collider other)
	{
		if (other.CompareTag ("Player"))
		{
			PlayerCollision playerCollision = other.gameObject.GetComponentInParent<PlayerCollision>();

			if (playerCollision && playerCollision.playerConfiguration.PlayerID == PlayerConfigurationManager.LocalPlayerId)
				return true;
		}

		return false;
	}
}
