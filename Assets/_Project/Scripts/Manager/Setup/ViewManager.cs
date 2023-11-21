using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Common.CommonData;

namespace Manager.Setup
{
    public class ViewManager : ManagerBase<ViewManager>
    {
        #region Singleton
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() => Get();
        #endregion

        #region VARIABLES

        //Private

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            Setup();
        }

        #endregion

        #region SETUP

        private void Setup()
        {
            OpenMainMenuScene();
        }

        #endregion

        #region FUNCTIONS

        public void OpenMainMenuScene()
        {
            print("OpenMainMenu");
            SceneManager.LoadSceneAsync(SceneName.MainMenuScene.ToString());
        }

        public void OpenGameScene()
        {
            print("OpenGameScene");
            SceneManager.LoadSceneAsync(SceneName.GameScene.ToString());
            SceneManager.LoadSceneAsync(SceneName.GameUIScene.ToString(), LoadSceneMode.Additive);
        }

        #endregion
    }
}