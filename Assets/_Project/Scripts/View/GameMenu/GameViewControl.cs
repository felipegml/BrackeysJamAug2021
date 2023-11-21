using Manager.Gameplay;
using Manager.Setup;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace View.GameMenu
{
    public class GameViewControl : MonoBehaviour
    {
        #region VARIABLESS

        [Header("UI")]
        public GameHUD gameHUD;
        public GameObject tutorialUI;
        public GamePauseUI gamePauseUI;
        public GameObject winUI;
        public TextMeshProUGUI winScoreTxt;
        public GameObject gameoverUI;
        public TextMeshProUGUI gameoverScoreTxt;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            Setup();

            //Add Events
            LevelManager.Pause_Event += LevelManager_Pause_Event;
            LevelManager.NextLevel_Event += LevelManager_NextLevel_Event;
            LevelManager.WinLevel_Event += LevelManager_WinLevel_Event;
            LevelManager.GameOver_Event += LevelManager_GameOver_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            LevelManager.Pause_Event -= LevelManager_Pause_Event;
            LevelManager.NextLevel_Event -= LevelManager_NextLevel_Event;
            LevelManager.WinLevel_Event -= LevelManager_WinLevel_Event;
            LevelManager.GameOver_Event -= LevelManager_GameOver_Event;
        }

        #endregion

        #region EVENTS

        private void LevelManager_Pause_Event(object[] obj = null)
        {
            UnPause_UI((bool)obj[0]);
        }

        private void LevelManager_NextLevel_Event(object[] obj = null)
        {
            ViewManager.Get().OpenGameScene();
        }

        private void LevelManager_WinLevel_Event(object[] obj = null)
        {
            winScoreTxt.text = PlayerManager.Get().totalScore.ToString();
            winUI.SetActive(true);
        }

        private void LevelManager_GameOver_Event(object[] obj = null)
        {
            gameoverScoreTxt.text = PlayerManager.Get().totalScore.ToString();
            gameoverUI.SetActive(true);
        }

        #endregion

        #region SETUP UI

        private void Setup()
        {
            tutorialUI.SetActive(false);
            gamePauseUI.gameObject.SetActive(false);
            winUI.SetActive(false);
            gameoverUI.SetActive(false);
        }

        public void RetryGame()
        {
            PlayerManager.Get().ResetLevel();
            ViewManager.Get().OpenGameScene();
        }

        public void QuitGame()
        {
            ViewManager.Get().OpenMainMenuScene();
        }

        //Pause UI
        public void UnPause_UI(bool _pause)
        {
            gamePauseUI.gameObject.SetActive(_pause);
        }

        #endregion
    }
}