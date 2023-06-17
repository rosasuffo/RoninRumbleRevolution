using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UISystem.Managers;
using System;

namespace UISystem.CharacterSelectScene
{
    public class JoinGameReady : NetworkBehaviour
    {
        public static JoinGameReady Instance { get; private set; }

        public event EventHandler OnPlayerReady;
        public event EventHandler OnAllPlayersReady;

        private Dictionary<ulong, bool> playersReadyDictionary;

        //private Dictionary<int, bool> characterVisibleForPlayer;

        private void Awake()
        {
            Instance = this;

            playersReadyDictionary = new Dictionary<ulong, bool>();
        }

        public void Start()
        {
            //GameMultiplayer.Instance.OnPlayerDataListChanged += GameMultiplayer_OnPlayerDataListChanged;
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //Hay q actualizar los cambios en los clientes tambn:
            SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

            playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            Debug.Log(serverRpcParams.Receive.SenderClientId);

            bool allClientsReady = true;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId])
                {
                    allClientsReady = false;
                    break;
                }
            }
            Debug.Log("All clients ready: " + allClientsReady);

            if (allClientsReady)
            {
                SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
                OnAllPlayersReady?.Invoke(this, EventArgs.Empty);
            }
        }

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId)
        {
            playersReadyDictionary[clientId] = true;

            OnPlayerReady?.Invoke(this, EventArgs.Empty);
        }


        /*
        [ClientRpc]
        private void SetCharacterClientRpc(ulong clientId)
        {
            playersChangeCharacterDictionary[clientId] = selectedSkin;
            OnNextCharacter?.Invoke(this, EventArgs.Empty);
        }
        
        [ClientRpc]
        private void SetPrevCharacterClientRpc(ulong clientId)
        {
            playersChangeCharacterDictionary[clientId] = selectedSkin;
            OnPreviousCharacter?.Invoke(this, EventArgs.Empty);
        }*/

        public bool IsPlayerReady(ulong clientId)
        {
            return playersReadyDictionary.ContainsKey(clientId) && playersReadyDictionary[clientId];
        }

        
    }
}
