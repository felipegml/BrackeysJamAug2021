using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class CommonData
    {
        #region EVENTS\TAGS

        //Custon event
        public delegate void CustomEvent(object[] obj = null);

        #endregion

        #region PATH/SCENE

        public enum SceneName
        {
            MainMenuScene,
            GameScene,
            GameUIScene
        }

        #endregion

        #region GAMEPLAY

        public enum GameplayTags
        {
            Player,
            PlayerAttack,
            Enemy,
            EnemyAttack,
            Wall,
            Shredder,
            Crystal,
            PowerUp
        }

        public static class GameplayValues
        {
            //Player
            public static float PLAYER_SPEED = 10f;
            public static float PLAYER_ATTACK_SPEED = 20f;
            public static float PLAYER_ATTACK_LIFETIME = 0.3f;
            public static float PLAYER_GODMODE_TIME = 5f;
            public static float PLAYER_SWORDUP_TIME = 5f;

            public static int STANDARD_LIVE = 5;

            //Player Animation time
            public static float PLAYER_UNLOCK_TIME = 1f;
            public static float PLAYER_TIME_ERASED = 2f;

            //Enemy
            public static float ENEMY_PREATTACK_TIME = 1f;
            public static float ENEMY_ATTACK_SPEED = 5f;
            public static float ENEMY_BULLET_LIFETIME = 10f;
            public static float ENEMY_DIE_TIME = .5f;

            //Level
            public static int RESTART_TIME = 3;
            public static float CRYSTAL_RESPAWN_TIME = 1f;
            public static int CRYSTAL_SCORE = 100;

            public static float CRYSTAL_DESTROY_TIME = 1f;
            public static int POWER_UP_NEED = 5;
        }

        public enum PlayerAnimTag
        {
            Walk,
            Dead,
            GetOut,
            GetIn
        }

        public enum PlayerState
        {
            Static,
            Control,
            NoDamage
        }

        public enum PowerUp
        {
            ShieldPowerUP,
            LiveUP,
            SwordUP,
            BombUP,
            DeflectUP
        }

        #endregion
    }
}