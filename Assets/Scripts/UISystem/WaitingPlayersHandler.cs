using System.Collections;
using System.Collections.Generic;
using UISystem.Managers;
using UnityEngine;

public class WaitingPlayersHandler : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel;
    //[SerializeField] private GameObject waitingPlayers;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            gameObject.SetActive(false);
            debugPanel.SetActive(false);
        }
    }
}
