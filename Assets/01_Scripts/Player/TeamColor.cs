using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerTeam))]
public class TeamColor : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb
    { get
        {
            if (_mpb is null) _mpb = new MaterialPropertyBlock();
            return _mpb;
        }
    }
    
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private List<Color> teamColors;

    private PlayerTeam _playerTeam;

    private void OnEnable()
    {
        _playerTeam = GetComponent<PlayerTeam>();
    }

    public void SetTeam(int teamIndex)
    {
        Mpb.SetColor("_BaseColor", teamColors[teamIndex]);
        targetRenderer.SetPropertyBlock(Mpb);
    }

    void Update()
    {
        SetTeam(_playerTeam.TeamIndex);
    }
}
