using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;

    private void Start()
    {
        //Esto seria si queremos mostrar a los jugadores en la pantalla de selected players
        GameMultiplayer.Instance.OnPlayerDataListChanged += GameMultiplayer_OnPlayerDataListChanged;

        UpdatePlayer();
    }

    private void GameMultiplayer_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {

    }

    private void UpdatePlayer()
    {
        //Ver si jugador esta conectado
        //if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        //{
        //    Show();
        //    //PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(playerIndex);
        //}
        //else Hide();

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
