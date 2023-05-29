using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UISystem.GameSceneUI
{
    public class GamePauseUI : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        //[SerializeField] private Button exitButton;

        private void Awake()
        {
            pauseButton.onClick.AddListener(() =>
            {
                Debug.Log("Paused");
                //El jugador se pausa solo para el jugador, el resto siguen
                GameManager.Instance.PauseGame();
                pauseButton.enabled = false;
            });

            resumeButton.onClick.AddListener(() =>
            {
                Debug.Log("Unpaused");
                //Reanudar
                GameManager.Instance.PauseGame();
                pauseButton.enabled = true;
            });

            /*
            exitButton.onClick.AddListener(() =>
            {
                GameManager.Instance.ExitGame();
            });*/
        }

        private void Start()
        {
            
            //Si hay un jugador pausado debemos mostrar la UI y ocultarla cuando salen
            GameManager.Instance.OnPlayerPaused += GameManager_OnPlayerPaused;
            GameManager.Instance.OnPlayerUnpaused += GameManager_OnPlayerUnpaused;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

            Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void GameManager_OnGamePausedCountdown(object sender, System.EventArgs e)
        {
            Show();
        }

        private void GameManager_OnPlayerPaused(object sender, System.EventArgs e)
        {
            Show();
        }

        private void GameManager_OnPlayerUnpaused(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"El jugador {clientId} ha abandonado la partida");
        }
    }
}
