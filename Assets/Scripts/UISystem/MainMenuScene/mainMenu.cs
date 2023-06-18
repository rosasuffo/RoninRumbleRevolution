using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UISystem.Managers;
using System;

namespace UISystems.MainMenuScene
{
    public class mainMenu : MonoBehaviour
    {
        //[SerializeField] private Button serverButton;
        //[SerializeField] private Button clientButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        //[SerializeField] private GameObject waitPanel;

        //private bool hostCreated = false;

        private void Awake()
        {
            

            /*
            serverButton.onClick.AddListener(() =>
            {
                GameMultiplayer.Instance.StartHost();
                hostCreated = true;
                SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);
            });
            clientButton.onClick.AddListener(() =>
            {
                GameMultiplayer.Instance.StartClient();

                if(!hostCreated) waitPanel.SetActive(true);
            });*/

            playButton.onClick.AddListener(() =>
            {
                SceneLoader.Load(SceneLoader.Scene.LobbyScene);
            });
        }

        private void Start()
        {
            //waitPanel.SetActive(false);
        }

    }
}
