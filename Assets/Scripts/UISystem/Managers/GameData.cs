using Movement.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.UISystem.Managers
{
    public class GameData : NetworkBehaviour
    {
        private GameObject info;
        //private List<Sprite> sprites;
        //private List<string> names;
        private List<string> hp;
        //private List<FighterMovement> fighterMovs;
        //[SerializeField] private Font fmfont;

        public void Start()
        {
            //StoreInfo();
            //ShowData();
        }

    }
}
