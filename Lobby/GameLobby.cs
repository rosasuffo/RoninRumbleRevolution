using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float heartbeatTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleHeartbeat(); //Que no se elimine el lobby tras pasar x tiempo para los quick joins

    }

    private void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f ) 
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void InitializeUnityAuthentication()
    {
        //Acceder solo una vez:
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            //Un id para cada jugador:
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(options);

            AuthenticationService.Instance.SignInAnonymouslyAsync();
        }    
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            Debug.Log("Creando lobby...");
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });
            Debug.Log("Lobby creado");
            GameMultiplayer.Instance.StartHost();
            //GameMultiplayer.Instance.StartClient();
            SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelect);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            GameMultiplayer.Instance.StartClient();

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            GameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
}
