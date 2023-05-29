using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UISystem.Managers;
using TMPro;
using UISystem.CharacterSelectScene;

namespace UISystem.GameSceneUI
{
    public class UIHandler : NetworkBehaviour
    {
        //Countdown
        //[SerializeField] private GameObject panelCountdown;
        [SerializeField] private TextMeshProUGUI Timer;

        private void Start()
        {
            //Cuenta atras:
            GameManager.Instance.OnFinishedCountdown += GameManager_OnFinishedCountdown;
        }
        private void Update()
        {
            Timer.text = (1*Mathf.RoundToInt(GameManager.Instance.GetCountdownToStartTimer())).ToString();
        }

        private void GameManager_OnFinishedCountdown(object sender, System.EventArgs e)
        {
            gameObject.SetActive(false);
        }


    }
}