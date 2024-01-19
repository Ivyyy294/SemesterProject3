using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    public Color Color => _teamColor;
    private Color _teamColor;
    
    [SerializeField] private List<Color> teamColors;

    private PlayerConfigurationContainer _playerConfigurationContainer;

    private void OnEnable()
    {
        _playerConfigurationContainer = GetComponentInChildren<PlayerConfigurationContainer>();
    }

    public void SetTeam(int teamIndex)
    {
        _teamColor = teamColors[teamIndex];
    }
    void Update()
    {
		SetTeam(_playerConfigurationContainer.TeamIndex);
    }
}
