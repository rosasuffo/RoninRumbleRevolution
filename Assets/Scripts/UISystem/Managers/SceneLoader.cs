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
            MainMenu,
            LoadingScene,
            CharacterSelect,
            GameScene,
            GameOver,
        }

        private static Scene targetScene;
        public static Action onLoaderCallBack;

        public static void Load(Scene targetScene)
        {
            SceneLoader.targetScene = targetScene;

            //SceneManager.LoadScene(Scene.LoadingScene.ToString());
            //SceneManager.LoadScene(targetScene.ToString());
            onLoaderCallBack = () =>
            {
                //Llamamos a la siguiente escena una vez hayamos terminado de cargar
                SceneManager.LoadScene(targetScene.ToString());
            };

            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        public static void LoadNetwork(Scene targetScene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        }

        public static void LoaderCallBack()
        {
            //Se produce despues de un Update para q permita a la pantalla refrescar
            //Ejecuta la accion para q llame a onLoaderCallBack q esta escuchando y se cargue la escena
            if(onLoaderCallBack != null)
            {
                onLoaderCallBack();
                onLoaderCallBack = null;
            }
            SceneManager.LoadScene(targetScene.ToString());
        }

        public static Scene ShowActiveScene()
        {
            return targetScene;
        }
    }
}
