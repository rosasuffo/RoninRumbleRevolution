using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem.Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UISystem.CharacterSelectScene
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> characterSkinsPrefabs;

        [SerializeField] private GameObject playerInterface;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI playerName;
        //[SerializeField] private TextMeshProUGUI lobbyName;
        //[SerializeField] private TextMeshProUGUI lobbyCode;


        [SerializeField] private int playerIndex; //para identificar los prefabs de la escena
        [SerializeField] private GameObject readyGameObject; //para señalizar al resto q el jugador esta ready

        //private List<GameObject> characterSkinsPrefabs;

        private void Awake()
        {
            if (NetworkManager.Singleton.IsServer) return;
            foreach (GameObject skin in characterSkinsPrefabs)
            {
                skin.SetActive(false);
            }
            characterSkinsPrefabs[0].SetActive(true);

            //BUTTONS:
            nextButton.onClick.AddListener(() =>
            {
                NextCharacterOption();
            });

            previousButton.onClick.AddListener(() =>
            {

                PreviousCharacterOption();
            });

            readyButton.onClick.AddListener(() =>
            {
                Debug.Log("Clicked");
                JoinGameReady.Instance.SetPlayerReady();
                nextButton.enabled = false;
                previousButton.enabled = false;
                readyButton.enabled = false;
            });

        }

        private void Start()
        {

            GameMultiplayer.Instance.OnPlayerDataListChanged += GameManager_OnPlayerDataListChanged;
            JoinGameReady.Instance.OnPlayerReady += JoinGameReady_OnPlayerChanged;
            JoinGameReady.Instance.OnAllPlayersReady += JoinGameReady_OnAllPlayersReady;

            //GameMultiplayer.Instance.UpdatePlayerName(playerIndex);
            //playerName.text = playerData.playerName.ToString();

            //JoinGameReady.Instance.OnNextCharacter += JoinGameReady_OnNextCharacter;
            //JoinGameReady.Instance.OnPreviousCharacter += JoinGameReady_OnPreviousCharacter;

            UpdatePlayer();

            //Lobby lobby = GameLobby.Instance.GetLobby();
            //lobbyName.text = "Lobby Name: " + lobby.Name;
            //lobbyCode.text = "Lobby Code: " + lobby.LobbyCode;
        }

        private void GameManager_OnPlayerDataListChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        private void JoinGameReady_OnPlayerChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        private void JoinGameReady_OnAllPlayersReady(object sender, EventArgs e)
        {
            GameMultiplayer.Instance.OnPlayerDataListChanged -= GameManager_OnPlayerDataListChanged;
        }

        /*
        private void JoinGameReady_OnChangeCharacter(object sender, EventArgs e)
        {
            characterSkinsPrefabs[selectedSkin].SetActive(false);
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            int newChar = JoinGameReady.Instance.ChangeCharacter(playerData.clientId);

            characterSkinsPrefabs[newChar].SetActive(true);
            Debug.Log("siguiente skin");
        }*/

        private void NextCharacterOption()
        {
            int selectedSkin = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex).characterIdFromList;
            characterSkinsPrefabs[selectedSkin].SetActive(false);
            selectedSkin++;
            if (selectedSkin > GameMultiplayer.Instance.GetCharacterListCount() - 1)
            {
                selectedSkin = 0;
            }
            GameMultiplayer.Instance.ChangePlayerCharacter(selectedSkin);
            Debug.Log("siguiente skin");
        }
        private void PreviousCharacterOption()
        {
            int selectedSkin = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex).characterIdFromList;
            characterSkinsPrefabs[selectedSkin].SetActive(false);
            selectedSkin--;
            if (selectedSkin < 0)
            {
                selectedSkin += GameMultiplayer.Instance.GetCharacterListCount();
            }
            GameMultiplayer.Instance.ChangePlayerCharacter(selectedSkin);
            Debug.Log("siguiente skin");
        }

        private void UpdatePlayer()
        {
            //Comprobar si el jugador al q le corresponde el prefab esta conectado
            if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
            {
                PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
                gameObject.SetActive(true);

                Debug.Log(playerData.playerName.ToString());
                playerName.text = playerData.playerName.ToString();
                
                //Si player esta ready se activa el mensaje
                readyGameObject.SetActive(JoinGameReady.Instance.IsPlayerReady(playerData.clientId));

                //characterSkinsPrefabs[GameMultiplayer.Instance.GetPlayerDataFromClientId(playerData.clientId).characterIdFromList].SetActive(true);
                //player.SetPlayerCharacter(GameMultiplayer.Instance.GetPlayerCharacter(playerIndex));
                foreach (var character in characterSkinsPrefabs)
                {
                    if (character == characterSkinsPrefabs[GameMultiplayer.Instance.GetPlayerDataFromClientId(playerData.clientId).characterIdFromList]) character.SetActive(true);
                    else character.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        


    }
}
