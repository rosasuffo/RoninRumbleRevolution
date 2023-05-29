using System.Threading;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIHandler_v2 : MonoBehaviour
    {
        public GameObject debugPanel;
        public Button hostButton; //el que acoge a todos los clientes (va a ser el play en unity, builds = clientes)
        public Button clientButton;

        private void Start()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            clientButton.onClick.AddListener(OnClientButtonClicked);
        }

        private void OnHostButtonClicked()
        {
            NetworkManager.Singleton.StartHost();
            debugPanel.SetActive(false);
        }

        private void OnClientButtonClicked()
        {
            NetworkManager.Singleton.StartClient();
            debugPanel.SetActive(false);
        }

    }
}

    /*UIHandlerInciial:
     * [SerializeField] private GameObject debugPanel;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;


        private void Start()
        {

            //loading.SetActive(false);
            hostButton.onClick.AddListener(() =>
            {
                Debug.Log("Click");
                GameMultiplayer.Instance.StartHost();
                debugPanel.SetActive(false);

            });

            clientButton.onClick.AddListener(() =>
            {
                GameMultiplayer.Instance.StartClient();
                debugPanel.SetActive(false);
            });
        }

        private void GameManager_OnStartInteractAction(object sender, System.EventArgs e)
        {
            GameManager.Instance.SetPlayerReady();
        }

        private void GameManager_OnPlayerReady(object sender, System.EventArgs e)
        {
            //if (GameManager.Instance.IsPlayerReady()) 
        }



        //public GameObject debugPanel;
        /*
        private void Start()
        {
            GameMultiplayer.Instance.StartHost();
            //GameMultiplayer.Instance.OnJoiningGame += GameMultiplayer_OnJoiningGame;
        }*/

    /*
    private void GameMultiplayer_OnJoiningGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }*/