using Manager.Gameplay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Common.CommonData;

namespace View.GameMenu
{
    public class GameHUD : MonoBehaviour
    {
        #region VARIABLES

        [Header("UI")]
        public TextMeshProUGUI liveTxt;
        public Transform heartContainer;
        public GameObject hearUp;
        public GameObject hearDown;
        public TextMeshProUGUI scoreTxt;
        public TextMeshProUGUI timeTxt;
        public TextMeshProUGUI levelTxt;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            Setup();

            //Add Events
            PlayerManager.UpdateLive_Event += PlayerManager_UpdateLive_Event;
            PlayerManager.UpdateScore_Event += PlayerManager_UpdateScore_Event;
            LevelManager.UpdateTime_Event += LevelManager_UpdateTime_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            PlayerManager.UpdateLive_Event -= PlayerManager_UpdateLive_Event;
            PlayerManager.UpdateScore_Event -= PlayerManager_UpdateScore_Event;
            LevelManager.UpdateTime_Event -= LevelManager_UpdateTime_Event;
        }

        #endregion

        #region EVENTS

        private void PlayerManager_UpdateScore_Event(object[] obj = null)
        {
            UpdateScore();
        }

        private void PlayerManager_UpdateLive_Event(object[] obj = null)
        {
            StartCoroutine(UpdateLive());
        }

        private void LevelManager_UpdateTime_Event(object[] obj = null)
        {
            UpdateTime((float)obj[0]);
        }

        #endregion

        #region SETUP

        private void Setup()
        {
            levelTxt.text = (PlayerManager.Get().currentLevel + 1).ToString();

            StartCoroutine(UpdateLive());
            UpdateScore();
        }

        #endregion

        #region FUNCTIONS

        private IEnumerator UpdateLive()
        {
            liveTxt.text = "LIVE: " + PlayerManager.Get().totalLive.ToString();

            foreach (Transform child in heartContainer)
                Destroy(child.gameObject);

            yield return new WaitForEndOfFrame();

            for(int i = 0; i < GameplayValues.STANDARD_LIVE; i++)
            {
                if (i >= PlayerManager.Get().totalLive)
                    Instantiate<GameObject>(hearUp, heartContainer);
                else
                    Instantiate<GameObject>(hearDown, heartContainer);
            }
        }

        private void UpdateScore()
        {
            scoreTxt.text = PlayerManager.Get().totalScore.ToString();
        }

        private void UpdateTime(float _time)
        {
            float _minutes = Mathf.FloorToInt(_time / 60); ;
            float _seconds = Mathf.FloorToInt(_time % 60);

            if (_time > 0)
                timeTxt.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
            else
                timeTxt.text = "00:00";
        }

        #endregion
    }
}