using System;
using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayer : NetworkBehaviour
{

    public static GameMultiplayer Instance { get; private set; }

    public event EventHandler OnJoiningGame;
    public event EventHandler OnPlayerDataListChanged; //necesitamos saber cuando cambia la lista == nuevo jugador

    private const int MAX_PLAYERS = 4;

    private NetworkList<PlayerData> playerDataNetworkList;

    //Hacer lista con todos para escoger entre ellos
    [SerializeField] private List<GameObject> characterPrefabs;
    

    //CLIENTE-SERVIDOR

    private void Awake() //Awake: primeros en ejecutarse pero no controlamos el orden
    {
        Instance = this;

        DontDestroyOnLoad(gameObject); 

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host creado");
    }

    public void StartClient()
    {
        //GameManager.Instance.OnStartInteractAction();
        //OnJoiningGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente nuevo conectado");
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {        
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game full";
            return;
        } 
        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Añadiendo jugador {clientId} a la partida");
        int id = playerDataNetworkList.Count + 1;
        //Cuando el cliente se conecta creamos nuevo PlayerData para guardar su info
        playerDataNetworkList.Add(new PlayerData { 
            clientId = clientId, 
            characterIdFromList = 0, //por defecto todos son el primero character
            playerName = $"Player{id}",
            
        });
        //Debug.Log($"Añadiendo jugador {clientId} a la partida");
    }

    //Para el prefab de character select, saber si para el id, existe un jugador
    public bool IsPlayerIndexConnected(int index)
    {
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
    public List<PlayerData> PlayersDataToList()
    {
        List<PlayerData> list = new List<PlayerData>();
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            list.Add(playerData);
        }
        return list;
    }

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

    public PlayerData GetPlayerDataFromGameObjectCharacter(GameObject fighter)
    {
        int playerIndex = -1;
        for(int i = 0; i < characterPrefabs.Count; i++)
        {
            if (characterPrefabs[i] == fighter.transform) playerIndex = i;
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

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.characterIdFromList = characterId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

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

}
