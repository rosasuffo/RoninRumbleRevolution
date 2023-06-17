using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem.Managers;
using UnityEngine;

public class GameRunningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private void Update()
    {
        float minutes = Mathf.FloorToInt(GameManager.Instance.GetTimer() / 60);
        float seconds = Mathf.FloorToInt(GameManager.Instance.GetTimer() % 60);

        timer.text = string.Format("{00:00}:{01:00}", minutes, seconds);
        //timer.text = currentTime.ToString();
    }
}
