using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DontdestroyOnLoadAccessor : MonoBehaviour
    {
        private static DontdestroyOnLoadAccessor _instance;
        public static DontdestroyOnLoadAccessor Instance
        {
            get
            {
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null) Destroy(this);
            this.gameObject.name = this.GetType().ToString();
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public GameObject[] GetAllRootsOfDontDestroyOnLoad()
        {
            return this.gameObject.scene.GetRootGameObjects();
        }
    }
}
