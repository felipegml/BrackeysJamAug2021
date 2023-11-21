using Manager.Gameplay;
using Manager.Setup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        #region VARIABLES

        [Header("UI")]
        public GameObject tutorialUI;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            OpenTutorial(false);
        }

        #endregion

        #region FUNCTIONS

        public void StartGameButton_Click()
        {
            PlayerManager.Get().ResetLevel();
            ViewManager.Get().OpenGameScene();
        }

        public void OpenTutorial(bool _show)
        {
            tutorialUI.SetActive(_show);
        }

        #endregion
    }
}