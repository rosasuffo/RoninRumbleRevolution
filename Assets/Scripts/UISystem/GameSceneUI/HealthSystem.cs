using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.GameSceneUI
{
    public class HealthSystem : NetworkBehaviour
    {
        public static HealthSystem Instance { get; set; }

        private Slider _privateLifebar;

        private void Start()
        {
            _privateLifebar = GetComponent<Slider>();
            _privateLifebar.gameObject.SetActive(true);
            //GameMultiplayer.Instance.OnPlayerDataListChanged += GameMultiplayer_OnPlayerDataListChanged;
            //GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        private void GameMultiplayer_OnPlayerDataListChanged(object sender, EventArgs e)
        {
            UpdateLifeClientRpc();
        }

        public void TakeHit()
        {
            UpdateLifebarClientRpc();
        }

        [ClientRpc]
        private void UpdateLifebarClientRpc()
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
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
