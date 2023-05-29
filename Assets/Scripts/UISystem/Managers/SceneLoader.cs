using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace UISystem.Managers
{
    public static class SceneLoader
    {
        public enum Scene
        {
            LoadingScene,
            CharacterSelect,
            GameScene
        }

        private static Scene targetScene;

        public static void Load(Scene targetScene)
        {
            SceneLoader.targetScene = targetScene;

            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        public static void LoadNetwork(Scene targetScene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        }

        public static void LoaderCallBack()
        {
            SceneManager.LoadScene(targetScene.ToString());
        }
    }
}
