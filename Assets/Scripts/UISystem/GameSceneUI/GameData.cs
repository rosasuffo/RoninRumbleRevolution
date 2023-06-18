using Movement.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UISystem.GameSceneUI;
using UnityEngine.Video;

public class GameData : NetworkBehaviour
{
    //private Dictionary<ulong, PlayerData> playersData;
    private GameObject info;
    private List<Sprite> sprites;
    private List<string> names;
    private List<string> hp;
    private List<FighterMovement> fighterMovs;
    [SerializeField] private Font fmfont;

    // AQUI SE CREA LA INFO QUE VA CAMBIANDO EN LA PARTIDA Y SE MUESTRA TANTO ESA COMO LA DEL PLAYERDATA
    public List<PlayerData> playersData;
    //public List<ulong> clientsId;
    private float timer;

    public Sprite huntress;
    public Sprite akaikaze;
    public Sprite oni;

    void Start()
    {
        playersData = GameMultiplayer.Instance.PlayersDataToList();
       
        StoreInfo();
        ShowData();
    }

    public void StoreInfo()
    {
        sprites = new List<Sprite>();
        names = new List<string>();
        hp = new List<string>();

        for (int i = 0; i < playersData.Count; i++)
        {
            Sprite sp;
            switch (playersData[i].characterIdFromList)
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
            names.Add($"{playersData[i].playerName}");            
            GameObject prefb = GameMultiplayer.Instance.GetCharacterPrefabFromPlayerDataIndex(playersData[i].clientId);
            FighterMovement fighterMov = prefb.GetComponent<FighterMovement>();
            fighterMovs.Add(fighterMov);
            hp.Add($"{playersData[i].playerLife}");
            
        }

    }
    public void ShowData()
    {
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
            float red = Random.Range(0f, 1f);
            float green = Random.Range(0f, 1f);
            float blue = Random.Range(0f, 1f);
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
            GameObject playerHP = new GameObject("PlayerHP");
            playerHP.transform.SetParent(canvas.transform, false);

            Text hps = playerHP.AddComponent<Text>();
            hps.text = hp[i];
            hps.font = fmfont;
            hps.color = color;
            hps.fontSize = 20;

            // Text position
            RectTransform rectHP = playerHP.GetComponent<RectTransform>();
            rectHP.position = imagePos + new Vector3(60, -15, 0);
            rectHP.sizeDelta = new Vector2(40, 22);

        }
    }
    void Update()
    {
        for (int i = 0; i < playersData.Count; i++)
        {
            hp[i] = $"{playersData[i].playerLife}";
        }
    }
}
