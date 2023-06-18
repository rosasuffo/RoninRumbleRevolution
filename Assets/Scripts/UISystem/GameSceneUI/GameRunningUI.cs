using Movement.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem.Managers;
using UnityEngine;
using UnityEngine.UI;

public class GameRunningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private GameObject info;
    private List<Sprite> sprites;
    private List<string> names;
    private List<string> hp;
    private List<FighterMovement> fighterMovs;
    [SerializeField] private Font fmfont;
    public List<PlayerData> playersData;
    public List<GameObject> playerHPs;

    public Sprite huntress;
    public Sprite akaikaze;
    public Sprite oni;

    public void Awake()
    {
        playersData = GameMultiplayer.Instance.PlayersDataToList();

        StoreInfo();
        ShowData();
    }
    private void Update()
    {
        float minutes = Mathf.FloorToInt(GameManager.Instance.GetTimer() / 60);
        float seconds = Mathf.FloorToInt(GameManager.Instance.GetTimer() % 60);

        timer.text = string.Format("{00:00}:{01:00}", minutes, seconds);
        //timer.text = currentTime.ToString();

    }
    public void UpdateLife(ulong clientId, int playerLife)
    {
        for (int i = 0; i < playersData.Count; i++)
        {
            if (playersData[i].clientId == clientId)
            {
                Text playerHP = playerHPs[i].GetComponent<Text>();
                playerHP.text = $"{playersData[i].playerLife}";
            }
        }
    }

    public void StoreInfo()
    {
        Console.WriteLine("STOREINFO");

        sprites = new List<Sprite>();
        names = new List<string>();
        hp = new List<string>();
        fighterMovs = new List<FighterMovement>();

        foreach (PlayerData player in playersData)
        {
            Sprite sp;
            switch (player.characterIdFromList)
            {
                case 0:
                    sp = huntress;
                    break;
                case 1:
                    sp = akaikaze;
                    break;
                default:
                    sp = oni;
                    break;
            }
            sprites.Add(sp);
            names.Add($"{player.playerName}");
            GameObject prefb = GameMultiplayer.Instance.GetCharacterPrefabFromPlayerDataIndex(player.clientId);
            FighterMovement fighterMov = prefb.GetComponent<FighterMovement>();
            fighterMovs.Add(fighterMov);
            hp.Add($"{player.playerLife}");

        }

    }
    public void ShowData()
    {
        Console.WriteLine("SHOWDATA");
        playerHPs = new List<GameObject>();

        // Canvas
        info = new GameObject("GameInfo");
        Canvas canvas = info.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        info.AddComponent<CanvasScaler>();
        info.AddComponent<GraphicRaycaster>();


        float x = (640 / playersData.Count) / 2;
        for (int i = 0; i < playersData.Count; i++)
        {
            Vector3 imagePos = new Vector3(x, 50, 0);
            x += 580 / playersData.Count;

            // Character Image
            GameObject imageObj = new GameObject("Image");
            imageObj.transform.SetParent(canvas.transform, false);
            imageObj.transform.position = imagePos;
            imageObj.transform.localScale = new Vector3(0.5f, 0.5f, 0);

            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprites[i];

            // Color Texto
            float red = UnityEngine.Random.Range(0f, 1f);
            float green = UnityEngine.Random.Range(0f, 1f);
            float blue = UnityEngine.Random.Range(0f, 1f);
            Color color = new Color(red, green, blue);

            // Player Name
            GameObject playerName = new GameObject("PlayerName");
            playerName.transform.SetParent(canvas.transform, false);

            Text pn = playerName.AddComponent<Text>();
            pn.text = names[i];
            pn.font = fmfont;
            pn.color = color;
            pn.fontSize = 15;

            // Text position
            RectTransform rectPN = playerName.GetComponent<RectTransform>();
            rectPN.position = imagePos + new Vector3(60, 15, 0);
            rectPN.sizeDelta = new Vector2(40, 22);

            // Player HP
            GameObject playerHP = new GameObject($"PlayerHP{i}");
            playerHP.transform.SetParent(canvas.transform, false);

            Text hps = playerHP.AddComponent<Text>();
            hps.text = hp[i];
            hps.font = fmfont;
            hps.color = color;
            hps.fontSize = 20;
            playerHPs.Add(playerHP);

            // Text position
            RectTransform rectHP = playerHP.GetComponent<RectTransform>();
            rectHP.position = imagePos + new Vector3(60, -15, 0);
            rectHP.sizeDelta = new Vector2(40, 22);

        }
    }
}
