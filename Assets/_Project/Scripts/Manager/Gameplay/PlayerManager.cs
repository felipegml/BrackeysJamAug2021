using Persona.Level;
using Persona.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Manager.Gameplay
{
    public class PlayerManager : ManagerBase<PlayerManager>
    {
        #region Singleton
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() => Get();
        #endregion

        #region VARIABLES

        [Header("Data")]
        public int currentLevel = 0;
        public int totalLive;
        public int totalScore = 0;

        //Events
        public static event CustomEvent UpdateLive_Event;
        public static event CustomEvent UpdateScore_Event;
        public static event CustomEvent GameOver_Event;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            Setup();

            //Add Events
            LevelManager.StartGame_Event += LevelManager_StartGame_Event;
            PlayerControl.Died_Event += PlayerControl_Died_Event;
            PlayerControl.PowerUp_Event += PlayerControl_PowerUp_Event;
            CrystalCapacitor.DestroyCrystal_Event += CrystalCapacitor_DestroyCrystal_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            LevelManager.StartGame_Event -= LevelManager_StartGame_Event;
            PlayerControl.Died_Event -= PlayerControl_Died_Event;
            PlayerControl.PowerUp_Event -= PlayerControl_PowerUp_Event;
            CrystalCapacitor.DestroyCrystal_Event -= CrystalCapacitor_DestroyCrystal_Event;
        }

        #endregion

        #region EVENTS

        private void LevelManager_StartGame_Event(object[] obj = null)
        {
            Setup();
        }

        private void PlayerControl_Died_Event(object[] obj = null)
        {
            VerifyLives();
        }

        private void PlayerControl_PowerUp_Event(object[] obj = null)
        {
            if ((PowerUp)obj[0] == PowerUp.LiveUP)
                LiveUP();
        }

        private void CrystalCapacitor_DestroyCrystal_Event(object[] obj = null)
        {
            UpdateScore();
        }

        #endregion

        #region SETUP

        private void Setup()
        {
            if(currentLevel == 0)
            {
                totalLive = GameplayValues.STANDARD_LIVE;
                totalScore = 0;

                UpdateLive_Event?.Invoke();
                UpdateScore_Event?.Invoke();
            }
        }

        public void NextLevel()
        {
            currentLevel++;
        }

        public void ResetLevel()
        {
            currentLevel = 0;
            totalLive = GameplayValues.STANDARD_LIVE;
            totalScore = 0;

            UpdateLive_Event?.Invoke();
            UpdateScore_Event?.Invoke();
        }

        #endregion

        #region FUNCTIONS

        private void VerifyLives()
        {
            totalLive--;

            bool _gameover = totalLive == 0;

            UpdateLive_Event?.Invoke();
            GameOver_Event?.Invoke(new object[] { _gameover } );
        }

        private void LiveUP()
        {
            totalLive++;

            if (totalLive > GameplayValues.STANDARD_LIVE)
                totalLive = GameplayValues.STANDARD_LIVE;

            UpdateLive_Event?.Invoke();
        }

        private void UpdateScore()
        {
            totalScore += GameplayValues.CRYSTAL_SCORE;
            UpdateScore_Event?.Invoke();
        }

        #endregion
    }
}