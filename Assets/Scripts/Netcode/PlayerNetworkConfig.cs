using Cinemachine;
using Movement.Components;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using InputSystems;
using UISystem.Managers;

namespace Netcode
{
    public class PlayerNetworkConfig : NetworkBehaviour
    {
        public static event EventHandler OnAnyPlayerSpawned;
        public static event EventHandler OnNewPlayerSpawned;

        public static void ResetStaticData()
        {
            OnAnyPlayerSpawned = null;
        }


        public static PlayerNetworkConfig LocalInstance { get; private set; }
        

        //public GameObject characterPrefab;
        //[SerializeField] private List<Vector3> spawnPositionList;

        private void Awake()
        {
            //PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
            //SetPlayerCharacter(GameMultiplayer.Instance.GetPlayerCharacter(playerData.characterId));
        }


        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            LocalInstance = this;
            
            transform.position = Vector3.zero;
            
            InstantiateCharacterServerRpc(OwnerClientId);

            OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

            //transform.position = Vector3.zero;
            //transform.position = spawnPositionList[(int)GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        }

        [ServerRpc]
        public void InstantiateCharacterServerRpc(ulong clientId)
        {
            
            GameObject characterGameObject = Instantiate(GameMultiplayer.Instance.GetCharacterPrefabFromPlayerDataIndex(clientId));
            characterGameObject.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            characterGameObject.transform.SetParent(transform, false);

            Debug.Log("Configurando personaje del jugador " + clientId);
        }
    }
}
