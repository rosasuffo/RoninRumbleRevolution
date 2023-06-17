using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UISystem.Managers;

namespace UYSystem
{
    public class HostDisconnected : MonoBehaviour
    {
        [SerializeField] private Button playAgainButton;
        [SerializeField] private GameObject winScreen;

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

            gameObject.SetActive(false);

            playAgainButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                SceneLoader.Load(SceneLoader.Scene.MainMenu);

            });
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            //Verificamos q sea el host
            if (clientId == NetworkManager.ServerClientId)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
