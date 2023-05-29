using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.GameSceneUI
{
    public class HealthSystem : MonoBehaviour
    {
        //[SerializeField] private Slider privateLifebar;
        //private Slider publicLifebar;

        private void Start()
        {
            //privateLifebar.gameObject.SetActive(false);
            //GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            //privateLifebar.gameObject.SetActive(true);
            //publicLifebar = gameObject.GetComponent<FighterNetworkConfig>().GetComponent<Slider>();
        }
    }
}
