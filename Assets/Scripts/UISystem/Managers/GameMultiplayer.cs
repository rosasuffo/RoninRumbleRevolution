using Movement.Components;
using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UISystem.GameSceneUI;
using UISystem.Managers;
using Unity.Collections;
using Unity.Netcode;
//using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameMultiplayer : NetworkBehaviour
{

    public static GameMultiplayer Instance { get; private set; }

    //public event EventHandler OnHostCreated;
    public event EventHandler OnJoiningGame;
    public event EventHandler OnPlayerDataListChanged; //necesitamos saber cuando cambia la lista == nuevo jugador
    //public event EventHandler OnNewPla
    //public event EventHandler OnPlayerUpdatePrivateLife;

    public const int MAX_PLAYERS = 4;
    //Clave para PlayerPrefabs:
    private const string PLAYER_PREFS_MULTIPLAYER = "PlayerMultiplayer";

    private NetworkList<PlayerData> playerDataNetworkList;
    private NetworkList<FixedString64Bytes> playersNamesNetworkList;
    private string playerName_config;
    //private readonly System.Object xLock = new System.Object(); 

    //Hacer lista con todos para escoger entre ellos
    [SerializeField] private List<GameObject> characterPrefabs;
    

    //CLIENTE-SERVIDOR

    private void Awake() //Awake: primeros en ejecutarse pero no controlamos el orden
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerName_config = "Player "+ UnityEngine.Random.Range(100, 10000).ToString();
        playersNamesNetworkList = new NetworkList<FixedString64Bytes>();

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

        
        //playersNamesNetworkList[0] = "Server";
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
    }


    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
        //OnHostCreated?.Invoke(this, EventArgs.Empty);
        Debug.Log("Host creado");
    }

    public void StartClient()
    {
        //playerName_config = playerName;
        //UpdatePlayerNameServerRpc(playerName);
        
        //GameManager.Instance.OnStartInteractAction();
        //OnJoiningGame?.Invoke(this, EventArgs.Empty);
        //NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente nuevo conectado");
    }


    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {        
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == MAX_PLAYERS + 1)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game full";
            return;
        } 
        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        playersNamesNetworkList.Add(new FixedString64Bytes("Player " + UnityEngine.Random.Range(100, 10000).ToString()));
        UpdatePlayerNameServerRpc(clientId, GetPlayerName());
        if (clientId == 0) return;
        Debug.Log($"Añadiendo jugador {clientId}, {playersNamesNetworkList[(int)clientId]} a la partida");
        //Cuando el cliente se conecta creamos nuevo PlayerData para guardar su info
        playerDataNetworkList.Add(new PlayerData { 
            clientId = clientId, 
            playerName = playersNamesNetworkList[(int)clientId],
            characterIdFromList = 0, //por defecto todos son el primero character
            playerLife = 100,
            
        });
        Debug.Log($"{playerDataNetworkList.Count} on PlayersIds list: ");
        foreach (var playerData in playerDataNetworkList)
        {
            Debug.Log($"{playerData.clientId}");
        }
        //Debug.Log($"Añadiendo jugador {clientId} a la partida");
    }

    /*
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
    }*/
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"El jugador {clientId} se ha desconectado.");
        //UpdateClients();
    }

    //Para el prefab de character select, saber si para el id, existe un jugador
    public bool IsPlayerIndexConnected(int index)
    {
        Debug.Log($"{index} < {playerDataNetworkList.Count}");
        //si el indice q le hemos dado esta por debajo del total de la lista quiere decir q esta conectado
        return index < playerDataNetworkList.Count;
    }




    /*
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId) return i;
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.clientId == clientId) { return playerData; }
        }
        return default;
    }

    public void ChangePlayerCharacter(int characterId)
    {
        ChangePlayerCharacterServerRpc(characterId);
    }

    [ServerRpc]
    private void ChangePlayerCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.characterId = characterId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }*/
    /*
    private int GetFirstUnusedCharacterId()
    {
        for(int i = 0; i < characterPrefabs.Count; i++)
        {
            return i;
        }
        return -1;
    }
    */

    public string GetPlayerName()
    {
        return playerName_config; 
    }

    public void SetPlayerName(string playerName)
    {
        Debug.Log("Configurar nombre: " + playerName);
        playerName_config = playerName;
    }



    //#region GESTION PLAYER DATA
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        //Devolvemos jugador de la lista q ocupa esa pos
        return playerDataNetworkList[playerIndex];
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData data in playerDataNetworkList)
        {
            if (data.clientId == clientId) return data;
        }
        return default;
    }

    public List<PlayerData> PlayersDataToList()
    {
        List<PlayerData> players = new List<PlayerData>();
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            players.Add(playerData);
        }
        return players;
    }

    public PlayerData GetPlayerDataFromGameObject(GameObject fighter)
    {
        int playerIndex = -1;
        for(int i = 0; i < characterPrefabs.Count; i++)
        {
            if (characterPrefabs[i] == fighter) playerIndex = i;
        }

        if (playerIndex > -1) return GetPlayerDataFromPlayerIndex(playerIndex);

        else return default;

    }

    public int GetPlayerIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId) return i;
        }
        return -1;
    }

    public GameObject GetCharacterPrefabFromPlayerDataIndex(ulong clientId)
    {
        int characterId = GetPlayerDataFromClientId(clientId).characterIdFromList;
        return characterPrefabs[characterId];
    }

    public void ChangePlayerCharacter(int character)
    {
        ChangePlayerCharacterServerRpc(character);
    }
    //#endregion

    //#region SERVER FUNCTIONS
    [ServerRpc]
    public void UpdatePlayerNameServerRpc(ulong clientId, string playerName)
    {
        Debug.Log("Hola?!");
        playersNamesNetworkList[(int)clientId] = playerName;
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.characterIdFromList = characterId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerDataServerRpc(PlayerData auxData)
    {
        int playerIndex = GetPlayerIndexFromClientId(auxData.clientId);
        PlayerData playerData = playerDataNetworkList[playerIndex];
        playerData = auxData;
        playerDataNetworkList[playerIndex] = playerData;

        //OnPlayerUpdatePrivateLife?.Invoke(this, EventArgs.Empty);
    }
    //#endregion

    /*
    [ClientRpc]
    private void ChangePlayerCharacterClientRpc(ulong clientId)
    {
        playersReadyDictionary[clientId] = true;

        OnPlayerReady?.Invoke(this, EventArgs.Empty);
    }*/



    //Devuelve el character de la lista en la posicion id
    public int GetCharacterListCount()
    {
        return characterPrefabs.Count;
    }

    public Scene ShowActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

}
