using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName_input;
    private string playerName;

    [SerializeField] private Button mainMenuButton;

    //Lobby
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private LobbyCreateUI lobbyCreatePanel;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInput;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.MainMenu);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            //GameMultiplayer.Instance.UpdatePlayerName("Server");
            lobbyCreatePanel.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            //GameMultiplayer.Instance.UpdatePlayerName(playerName_input.text);
            GameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            //GameMultiplayer.Instance.UpdatePlayerName(playerName_input.text);
            GameLobby.Instance.JoinWithCode(joinCodeInput.text);
        });    
    }

    private void Start()
    {
        playerName_input.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayer.Instance.SetPlayerName(newText);

        });

    }
}
