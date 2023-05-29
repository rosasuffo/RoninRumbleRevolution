using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnected : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        gameObject.SetActive(false);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //Verificamos q sea el host
        if(clientId == NetworkManager.ServerClientId)
        {
            gameObject.SetActive(true);
        }
    }
}
