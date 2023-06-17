using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using InputSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Netcode;
using UnityEngine.UI;

namespace UISystem.Managers
{
    public class GameManager: NetworkBehaviour
    {
        //ESTADOS

        public static GameManager Instance { get; private set; }

        public event EventHandler OnStateChanged;
        public event EventHandler OnPlayerReady;
        public event EventHandler OnStartInteractAction;

        public event EventHandler OnStartGame;
        public event EventHandler OnFinishedCountdown;
        public event EventHandler OnPlayerPaused;
        public event EventHandler OnPlayerUnpaused;
        //public event EventHandler OnPlayerExitGame;

        public event EventHandler OnOnePlayerRest;

        private enum State
        {
            CountdownToStart,
            GamePlaying,
            GameOver,
        }

        [SerializeField] private GameObject playerPrefab;
        //[SerializeField] private Slider publicLifeBar;
        //[SerializeField] private CinemachineVirtualCamera virtualCamera;


        private NetworkVariable<State> state = new NetworkVariable<State>(State.CountdownToStart);
        private bool isPlayerReady;
        private bool isPlayerPaused = false;
        //private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
        //private NetworkVariable<bool> isOnePlayerPaused = new NetworkVariable<bool>(false); //jugador se para para todos (aunq solo el vea su interfaz)

        private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
        private Dictionary<ulong, bool> playersReadyDictionary;

        private NetworkVariable<float> timer = new NetworkVariable<float>(0f);
        private float timerMaxDuration = 3f * 60f;
        //private Dictionary<ulong, bool> playersPausedDictionary;

        private void Awake()
        {
            Instance = this;

            playersReadyDictionary = new Dictionary<ulong, bool>();
            //playersPausedDictionary = new Dictionary<ulong, bool>();
        }

        private void Start()
        {
            //PlayerNetworkConfig.LocalInstance.OnAnyPlayerSpawned += Player_OnNewPlayer;
            //OnStartGame?.Invoke(this, EventArgs.Empty);
            //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }

        public override void OnNetworkSpawn()
        {
            state.OnValueChanged += State_OnValueChanged;
            //isOnePlayerPaused.OnValueChanged += IsOnePlayerPaused_OnValueChanged;

            

            if (IsServer)
            {
                //InstantiatePlayerServerRpc(OwnerClientId);
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
                //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
                

            }

            
            //ICinemachineCamera virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
            /*
            if (!IsCountdownToStartActive())
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            }*/
        }

        private void State_OnValueChanged(State previousValue, State newValue)
        {
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }

        

        /*
        private void IsGamePaused_OnValueChange(bool previousValue, bool newValue)
        {
            if(isGamePaused.Value)
            {
                Time.timeScale = 0f; //parar jugeo
            }
            else
            {
                Time.timeScale = 1f; //reanudar jugeo
            }
        }*/

        private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            foreach(ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                //SetPlayerReady();
                InstantiatePlayerServerRpc(id);
            }
        }

        [ServerRpc]
        public void InstantiatePlayerServerRpc(ulong clientId)
        {
            GameObject playerGameObject = Instantiate(playerPrefab);
            playerGameObject.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            playerGameObject.transform.SetParent(transform, false);

            Debug.Log("Carga juego completada: nuevo player en la partida");
        }




        /*
                public void SetPlayerReady()
                {
                    if (state.Value == State.WaitingToStart)
                    {
                        isPlayerReady = true;

                        SetPlayerReadyServerRpc();

                        OnPlayerReady?.Invoke(this, EventArgs.Empty);
                    }
                }

                [ServerRpc(RequireOwnership = false)]
                private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
                {
                    playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
                    Debug.Log(serverRpcParams.Receive.SenderClientId);

                    bool allClientsReady = true;
                    foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                    {
                        if(!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId]) 
                        {
                            allClientsReady = false;
                            break;
                        }
                    }
                    Debug.Log("All clients ready: " + allClientsReady);

                    if (allClientsReady)
                    {
                        state.Value = State.CountdownToStart;
                    }
                }*/

        //Getters y setters
        public bool IsCountdownToStartActive()
        {
            return state.Value == State.CountdownToStart;
        }

        public float GetCountdownToStartTimer()
        {
            return countdownToStartTimer.Value;
        }

        public void SetIsPlayerReady(bool ready) { isPlayerReady = ready; }
        public bool IsPlayerReady()
        {
            return isPlayerReady;
        }

        public bool IsGamePlaying()
        {
            return state.Value == State.GamePlaying;
        }  
        
        public float GetTimer()
        {
            return 1 * Mathf.RoundToInt(timer.Value);
        }

        public bool IsGameOver()
        {
            return state.Value == State.GameOver;
        }


        public void PauseGame()
        {
            isPlayerPaused = !isPlayerPaused; //se establece el estado contrario
            
            //El Rpc gestiona la pausa en todos
            if(isPlayerPaused)
            {
                OnPlayerPaused?.Invoke(this, EventArgs.Empty); //enevto q diga q un jugador ha pausado
            } else
            {
                OnPlayerUnpaused?.Invoke(this, EventArgs.Empty); 
            }
        }
        /*
        public void ExitGame()
        {
            SceneLoader.Load(SceneLoader.Scene.MainScene);
        }*/
        /*
        public void ExitGame()
        {
            OnPlayerExitGame?.Invoke(this, EventArgs.Empty);
        }*/

        /*
        [ServerRpc]
        private void PausePlayerServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //playersPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
            isOnePlayerPaused.Value = true;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playersPausedDictionary.ContainsKey(clientId) || !playersPausedDictionary[clientId])
                {
                    //En cuanto uno este pausado, devolvemos para q todos se pausen
                    isGamePaused.Value = true;
                    return;
                }

                //Si llega hasta aqui quiere decir q nadie esta pausado
                isGamePaused.Value = false;
            }

        }*/

        /*
        [ServerRpc]
        private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            playersPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playersPausedDictionary.ContainsKey(clientId) || !playersPausedDictionary[clientId])
                {
                    //En cuanto uno este pausado, devolvemos para q todos se pausen
                    isGamePaused.Value = true;
                    return;
                }

                //Si llega hasta aqui quiere decir q nadie esta pausado
                isGamePaused.Value = false;
            }
            
        }

        [ServerRpc]
        private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            playersPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        }*/

        private void Update()
        {
            if (!IsServer) return;

            switch (state.Value)
            {
                case State.CountdownToStart:
                    //Time.timeScale = 0f;
                    countdownToStartTimer.Value -= Time.deltaTime;
                    if(countdownToStartTimer.Value < 0f)
                    {
                        //Time.timeScale = 1f;
                        CountDownFinishedServerRpc(); //avisar a los clientes
                        state.Value = State.GamePlaying;
                        timer.Value = timerMaxDuration;
                    }
                    break;
                case State.GamePlaying:
                    Debug.Log("GamePlaying");
                    Debug.Log(timer.Value);
                    timer.Value -= Time.deltaTime;
                    if (timer.Value < 0f) state.Value = State.GameOver;
                    break;
                case State.GameOver:
                    break;
            }

            
            //Comprobar numero de jugadores
            if(NetworkManager.Singleton.ConnectedClients.Count == 1)
            {
                Debug.Log($"Todos los jugadores se han desconectado. Enhorabuena {NetworkManager.Singleton.ConnectedClientsIds[0]} has ganado");
                OnOnePlayerRest?.Invoke(this, EventArgs.Empty);
            }
        }

        [ServerRpc]
        private void CountDownFinishedServerRpc(ServerRpcParams serverRpcParams = default)
        {
            CountdownFinishedClientRpc(serverRpcParams.Receive.SenderClientId);
        }

        [ClientRpc]
        private void CountdownFinishedClientRpc(ulong clientId)
        {
            OnFinishedCountdown?.Invoke(this, EventArgs.Empty);
        }

        /*
        public Slider GetSliderPrefabForLifebar()
        {
            return publicLifeBar;
        }*/

        /*
        public override void OnNetworkDespawn()
        {
            if(IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            }
        }

        private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {

        }*/
    }
}
