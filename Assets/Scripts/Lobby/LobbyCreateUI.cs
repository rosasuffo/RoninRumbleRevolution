using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createButton;
    //[SerializeField] private TMP_InputField lobbyNameInputField;

    private void Start()
    {
        Hide();
    }

    private void Awake()
    {
        createButton.onClick.AddListener(() =>
        {
            Debug.Log("Crear lobby");
            string name = "Room";// + Random.Range(0, 1000).ToString();
            GameLobby.Instance.CreateLobby(name, false);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
