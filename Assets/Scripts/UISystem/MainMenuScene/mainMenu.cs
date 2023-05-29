using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UISystem.Managers;

namespace UISystems.MainMenuScene
{
    public class mainMenu : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        private void Start()
        {
            hostButton.onClick.AddListener(() =>
            {
                GameMultiplayer.Instance.StartHost();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelect);
            });
            clientButton.onClick.AddListener(() =>
            {
                GameMultiplayer.Instance.StartClient();
            });
        }

    }
}
