using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.GameSceneUI
{
    public class HealthSystem : NetworkBehaviour
    {
        public static HealthSystem Instance { get; private set; }

        [SerializeField] private Slider _privateLifebar;

        private void Start()
        {
            //_privateLifebar = gameObject.GetComponent<Slider>();
            _privateLifebar.gameObject.SetActive(true);
            GameMultiplayer.Instance.OnPlayerDataListChanged += GameMultiplayer_OnPlayerDataListChanged;
            //OnPlayerUpdatePrivateLife += HealthSystem_OnPlayerUpdatePrivateLife;
            //GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        private void HealthSystem_OnPlayerUpdatePrivateLife(ulong clientId, object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GameMultiplayer_OnPlayerDataListChanged(object sender, EventArgs e)
        {
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                UpdateLifebarClientRpc(id);
            }
        }

        public void UpdatePrivateLifebar(int playerLife)
        {
            Debug.Log("Damage");
            //PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
            //if(playerData.playerLife <= 0)
            //{
            //    Die();
            //}
            _privateLifebar.value = playerLife;
        }

        [ClientRpc]
        private void UpdateLifebarClientRpc(ulong clientId)
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
            _privateLifebar.value = playerData.playerLife;
        }


        [ClientRpc]
        private void UpdateLifeClientRpc()
        {
            foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
                _privateLifebar.value = playerData.playerLife;
            }
            
        }

    }
}
