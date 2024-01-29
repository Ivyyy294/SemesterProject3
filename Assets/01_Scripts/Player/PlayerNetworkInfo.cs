using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkInfo : MonoBehaviour
{
    public bool IsPlayer => _playerInput.Owner;
    private PlayerInputProcessing _playerInput;

    private void OnEnable()
    {
        _playerInput = GetComponentInChildren<PlayerInputProcessing>();
    }
}
