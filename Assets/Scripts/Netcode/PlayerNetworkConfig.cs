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
using UnityEngine.UI;
using TMPro;

namespace Netcode
{
    public class PlayerNetworkConfig : NetworkBehaviour
    {
        public static PlayerNetworkConfig LocalInstance { get; private set; }

        public static event EventHandler OnAnyPlayerSpawned;
        public static event EventHandler OnNewPlayerSpawned;

        public static void ResetStaticData()
        {
            OnAnyPlayerSpawned = null;
        }

        //public GameObject characterPrefab;
        [SerializeField] private GameObject _UIPanelPrefab;
        private Canvas _canvasScene;
        //[SerializeField] private List<Vector3> spawnPositionList;

        private void Awake()
        {
            _canvasScene = GameObject.FindGameObjectWithTag("UIPlayer").GetComponent<Canvas>();
            //_canvasScene.GetComponent<Canvas>();
            //PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
            //SetPlayerCharacter(GameMultiplayer.Instance.GetPlayerCharacter(playerData.characterId));
            //_privateLifebar = GetComponentInChildren<Canvas>().GetComponentInChildren<Slider>();
        }


        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;

                transform.position = Vector3.zero;

                InstantiateCharacterServerRpc(OwnerClientId);

                OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

                if (OwnerClientId != 0)
                {
                    InstantiatePanel(OwnerClientId);
                }
            }
            
            
            

            

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

        public void InstantiatePanel(ulong clientId)
        {
            GameObject panelUI = Instantiate(_UIPanelPrefab, _canvasScene.transform);
            panelUI.GetComponentInChildren<TextMeshProUGUI>().text = clientId.ToString();
            //panelUI.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            //characterGameObject.transform.SetParent(transform, false);

            Debug.Log("UI ready");
        }


    }
}
