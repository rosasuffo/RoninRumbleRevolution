using System;
using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinnerUI : MonoBehaviour
{
    //[SerializeField] private TextMesh playerId;
    [SerializeField] private Button playAgain;
    [SerializeField] private Button mainMenu;

    private void Awake()
    {
        playAgain.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenu);
        });

        mainMenu.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenu);
        });
    }

    private void Start()
    {
        gameObject.SetActive(false);

        GameManager.Instance.OnOnePlayerRest += GameManager_OnOnePlayerRest;
    }

    private void GameManager_OnOnePlayerRest(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
    }
}
