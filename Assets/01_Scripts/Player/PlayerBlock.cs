using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
	[SerializeField] GameObject playerBlockCollider;

	PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = transform.parent.GetComponentInChildren<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
		bool block = playerInput.BlockPressed;

		if (playerBlockCollider.activeInHierarchy != block)
			playerBlockCollider.SetActive (block);
    }
}
