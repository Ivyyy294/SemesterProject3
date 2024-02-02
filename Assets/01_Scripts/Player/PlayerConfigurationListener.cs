using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ivyyy.Network;
using UnityEngine;
using UnityEngine.Events;


public class PlayerConfigurationListener : MonoBehaviour
{
    public static PlayerConfigurationListener Me { get; private set; }

    public UnityEvent onConfigurationChange;

    private int[] _playerTeams;
    private PlayerConfiguration[] _playerConfigs;
    private bool[] _playerVariationLookup;
    
    public bool LookUpVariation(int playerID) => _playerVariationLookup[playerID];

    private readonly int _teamCount = 2;

    private void Awake()
    {
        if (Me != null)
        {
            Destroy(this);
            return;
        }
        Me = this;
    }

    private void OnEnable()
    {
        _playerConfigs = PlayerConfigurationManager.Me.playerConfigurations;
        _playerTeams = _playerConfigs.Select(_=>-1).ToArray();
        _playerVariationLookup = _playerTeams.Select(_ => false).ToArray();
    }

    private void Update()
    {
        var log = "";
        var hasChanges = false;
        foreach (var c in _playerConfigs)
        {
            log += $"[{c.connected}, {c.teamNr}]\n";
            var currentConnected = c.connected;
            var currentTeam = currentConnected ? c.teamNr : -1;
            if (currentTeam != _playerTeams[c.playerId])
            {
                hasChanges = true;
                _playerTeams[c.playerId] = currentTeam;
            }
        }
        if(hasChanges) OnConfigChanges();
    }

    private void OnConfigChanges()
    {
        Debug.LogWarning("Changes!!!");
        bool[] variationAssigned = new bool[_teamCount];
        for (int i = 0; i < _playerTeams.Length; i++)
        {
            var currentTeam = _playerTeams[i];
            if (currentTeam == -1)
            {
                _playerVariationLookup[i] = false;
                continue;
            }
            var variationWasAssigned = variationAssigned[currentTeam];
            _playerVariationLookup[i] = !variationWasAssigned;
            variationAssigned[currentTeam] = true;
        }
        onConfigurationChange.Invoke();
    }

    private void OnDestroy()
    {
        if (Me == this)
        {
            Me = null;
        }
    }
}
