using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    private void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();
        lobbyName.text = "Lobby Name: " + lobby.Name;
        lobbyCode.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
