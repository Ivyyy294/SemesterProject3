using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerNameTagManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI nameTagTemplate;
    [SerializeField] private TeamColorSettings teamColors;
    
    public static PlayerNameTagManager Me { get; private set; }
    private GameObject[] _players;
    private PlayerConfiguration[] _playerConfigs;
    private TextMeshProUGUI[] _playerNameTags;

    private bool _activated;

    private void Awake()
    {
        if (Me != null)
        {
            Destroy(this);
            return;
        }

        Me = this;
    }

    private void OnDestroy()
    {
        if (Me == this) Me = null;
    }
    
    private void OnEnable()
    {
        TryInit();
        if (PlayerConfigurationManager.Me == null)
        {
            Debug.LogWarning($"PlayerNameTagManager inactive without PlayerConfigurationManager");
        }
    }

    private void TryInit()
    {
        if (_activated) return;
        if (PlayerManager.Me == null) return;
        if (PlayerConfigurationManager.Me == null) return;
        
        _activated = true;
        var managerTransform = PlayerManager.Me.transform;
        _players = new GameObject[managerTransform.childCount];
        _playerNameTags = new TextMeshProUGUI[_players.Length];
        _playerConfigs = new PlayerConfiguration[_players.Length];
        for (int i = 0; i < managerTransform.childCount; i++)
        {
            _players[i] = managerTransform.GetChild(i).gameObject;
            var newTag = Instantiate(nameTagTemplate, canvas.transform);
            newTag.gameObject.name = $"PlayerTag {i}";
            _playerNameTags[i] = newTag;
            _playerConfigs[i] = PlayerConfigurationManager.Me.playerConfigurations[i];
        }

        UpdateNameTags();
    }

    private void Update()
    {
        TryInit();
        if(_activated) UpdateNameTags();
    }

    private void UpdateNameTags()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            var player = _players[i];
            var text = _playerNameTags[i];
            var config = _playerConfigs[i];

            var isLocalPlayer = player == PlayerManager.LocalPlayer;
            
            var isShown = player.activeInHierarchy && !isLocalPlayer;
            text.gameObject.SetActive(isShown);
            if (!isShown) continue;
            
            text.rectTransform.position = player.transform.position + Vector3.up;
            text.rectTransform.LookAt(CameraSystem.Me.MainCamera.transform.position);
            text.rectTransform.Rotate(0, 180, 0);
            text.text = config.playerName;
            text.color = teamColors.GetTeamColor(config.teamNr);
        }
    }

    private void OnDisable()
    {
        foreach (var text in _playerNameTags)
        {
            Destroy(text);
        }
    }
}
