using Manager.Gameplay.Data;
using Manager.Setup;
using Persona.Enemy;
using Persona.Level;
using Persona.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Common.CommonData;

namespace Manager.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        #region VARIABLES

        [Header("Data")]
        public List<Level_SO> levels = new List<Level_SO>();
        public List<GameObject> powerUps = new List<GameObject>();

        [Space]

        [Header("Components")]
        [Header("Level")]
        public Transform gridContainer;
        public Transform levelContainer;
        public Transform powerUpContainer;

        [Header("Crystal")]
        public GameObject crystalPrefab;
        public Transform crystalContainer;

        [Header("Player Setup")]
        public PlayerControl playerPrefab;
        public Transform playerContainer;
        public Transform attackPlayerContainer;

        [Header("Enemy Setup")]
        public Transform enemyContainer;
        public Transform attackEnemyContainer;

        //Private - Level
        private LevelData level;
        private int currentCrystalPortal = 0;
        private int totalCrystalDestroyed = 0;

        private bool paused = false;
        private bool locked = true;
        private float currentTime = 0;

        //Private - Player
        private PlayerControl player;
        private int currentPowerUP = 0;

        //Events
        public static event CustomEvent StartGame_Event;
        public static event CustomEvent Pause_Event;
        public static event CustomEvent UpdateTime_Event;
        public static event CustomEvent NextLevel_Event;
        public static event CustomEvent WinLevel_Event;
        public static event CustomEvent GameOver_Event;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            //Setup
            Time.timeScale = 1;

            SetupLevel();
            StartCoroutine(StartGame());

            //Add Events
            PlayerManager.GameOver_Event += PlayerManager_GameOver_Event;
            EnemyPortal.SpawnEnemy_Event += EnemyPortal_SpawnEnemy_Event;
            PlayerControl.Died_Event += PlayerControl_Died_Event;
            PlayerControl.PowerUp_Event += PlayerControl_PowerUp_Event;
            CrystalCapacitor.DestroyCrystal_Event += CrystalCapacitor_DestroyCrystal_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            PlayerManager.GameOver_Event -= PlayerManager_GameOver_Event;
            EnemyPortal.SpawnEnemy_Event -= EnemyPortal_SpawnEnemy_Event;
            PlayerControl.Died_Event -= PlayerControl_Died_Event;
            PlayerControl.PowerUp_Event -= PlayerControl_PowerUp_Event;
            CrystalCapacitor.DestroyCrystal_Event -= CrystalCapacitor_DestroyCrystal_Event;
        }

        void Update()
        {
            if(!locked)
            {
                GetInput();
                UpdateLevelTime();
            }
        }

        #endregion

        #region EVENTS

        private void PlayerManager_GameOver_Event(object[] obj = null) 
            => StartCoroutine(VerifyGameover((bool)obj[0]));

        private void EnemyPortal_SpawnEnemy_Event(object[] obj = null) 
            => SpawnEnemy((EnemyPortal)obj[0]);

        private void PlayerControl_Died_Event(object[] obj = null) 
            => StartCoroutine(PauseEverything(false));

        private void PlayerControl_PowerUp_Event(object[] obj = null)
        {
            if ((PowerUp)obj[0] == PowerUp.BombUP)
                DestroyAllBullets();
        }

        private void CrystalCapacitor_DestroyCrystal_Event(object[] obj = null) 
            => StartCoroutine(SpawnCrystal());

        #endregion

        #region SETUP

        private void ClearArea()
        {
            foreach (Transform child in playerContainer)
                Destroy(child.gameObject);

            foreach (Transform child in enemyContainer)
                Destroy(child.gameObject);

            foreach (Transform child in attackEnemyContainer)
                Destroy(child.gameObject);
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSeconds(1f);
            SpawnPlayer();

            locked = false;

            StartGame_Event?.Invoke(new object[] { PlayerManager.Get().currentLevel });

            yield return new WaitForSeconds(1f);
            SetupPortals();

            yield return new WaitForSeconds(1f);
            StartCoroutine(SpawnCrystal());
        }

        private IEnumerator VerifyGameover(bool _gameover)
        {
            yield return new WaitForEndOfFrame();

            if (!_gameover)
            {
                StartCoroutine(RespawnPlayer());
                yield return new WaitForSeconds(GameplayValues.RESTART_TIME);
                ResumeEverything(false);
            }
            else
                StartCoroutine(GameOver());
        }

        private void UpdateLevelTime()
        {
            if(!paused)
            {
                if (currentTime > 0)
                    currentTime -= Time.deltaTime;
                else
                    Timeout();
            }

            UpdateTime_Event?.Invoke(new object[] { currentTime } );
        }

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                UnPause();
        }

        private void UnPause()
        {
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;

            if (paused)
                StartCoroutine(PauseEverything(true));
            else
                ResumeEverything(true);

            Pause_Event?.Invoke(new object[] { paused });
        }

        private void Timeout()
        {
            StartCoroutine(PauseEverything(false));
            StartCoroutine(NextLevel());
        }

        private IEnumerator NextLevel()
        {
            StartCoroutine(player.NextLevel());
            yield return new WaitForSeconds(1f);
            PlayerManager.Get().NextLevel();

            yield return new WaitForEndOfFrame();

            if (PlayerManager.Get().currentLevel < levels.Count)
                NextLevel_Event?.Invoke();
            else
                WinLevel_Event?.Invoke();
        }

        private IEnumerator GameOver()
        {
            StartCoroutine(PauseEverything(false));
            yield return new WaitForSeconds(GameplayValues.PLAYER_TIME_ERASED);
            GameOver_Event?.Invoke();
        }

        #endregion

        #region LEVEL SETUP

        private void SetupLevel()
        {
            ClearArea();

            //Clean Level
            level = null;

            foreach (Transform child in gridContainer)
                Destroy(child.gameObject);

            foreach (Transform child in levelContainer)
                Destroy(child.gameObject);

            //Add Grid\level
            Instantiate<GameObject>(levels[PlayerManager.Get().currentLevel].levelGrid, gridContainer);

            level = Instantiate<LevelData>(levels[PlayerManager.Get().currentLevel].levelData, levelContainer);
            level.name = levels[PlayerManager.Get().currentLevel].levelData.name;

            currentTime = levels[PlayerManager.Get().currentLevel].levelTime;
            UpdateTime_Event?.Invoke(new object[] { currentTime });
        }

        private IEnumerator PauseEverything(bool _pause)
        {
            if(!_pause)
                locked = true;

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < level.enemyPortals.Count; i++)
                level.enemyPortals[i].Pause();

            foreach (Transform _bullet in attackEnemyContainer)
                _bullet.GetComponent<EnemyBullet>().Pause();

            foreach (Transform _enemy in enemyContainer)
                _enemy.GetComponent<EnemyControl>().Pause();
        }

        private void ResumeEverything(bool _pause)
        {
            if (!_pause)
                locked = false;

            for (int i = 0; i < level.enemyPortals.Count; i++)
                level.enemyPortals[i].Resume();

            foreach (Transform _bullet in attackEnemyContainer)
                _bullet.GetComponent<EnemyBullet>().Resume();

            foreach (Transform _enemy in enemyContainer)
                _enemy.GetComponent<EnemyControl>().Resume();
        }

        #endregion

        #region PLAYER SETUP

        private void SpawnPlayer(bool _revive = false)
        {
            player = null;

            foreach (Transform child in playerContainer)
                Destroy(child.gameObject);

            Vector2 _pos = level.playerPortal.transform.position;

            player = Instantiate<PlayerControl>(playerPrefab, _pos, Quaternion.identity, playerContainer);
            player.name = playerPrefab.name;
            player.Setup(attackPlayerContainer, _revive);
        }

        private IEnumerator RespawnPlayer()
        {
            yield return new WaitForSeconds(GameplayValues.PLAYER_TIME_ERASED);
            SpawnPlayer(true);
        }

        #endregion

        #region ENEMY SETUP

        private void SetupPortals()
        {
            for (int i = 0; i < level.enemyPortals.Count; i++)
                level.enemyPortals[i].Setup();
        }

        private void SpawnEnemy(EnemyPortal _portal)
        {
            EnemyControl _enemy = Instantiate<EnemyControl>(_portal.GetEnemyPrefab(),
                                                            _portal.transform.position,
                                                            Quaternion.identity,
                                                            enemyContainer);

            _enemy.Setup(attackEnemyContainer, _portal);
        }

        #endregion

        #region CRYSTAL CONTROL

        private IEnumerator SpawnCrystal()
        {
            totalCrystalDestroyed++;

            if(totalCrystalDestroyed >= GameplayValues.POWER_UP_NEED)
            {
                totalCrystalDestroyed = 0;
                SpawnPowerUp();
            }

            yield return new WaitForSeconds(GameplayValues.CRYSTAL_RESPAWN_TIME);

            List<int> _indexs = new List<int>();
            for (int i = 0; i < level.crystalPortals.Count; i++)
                _indexs.Add(i);

            _indexs.RemoveAt(currentCrystalPortal);

            int _index = Random.Range(0, _indexs.Count);
            currentCrystalPortal = _index;

            GameObject _crystal = Instantiate<GameObject>(crystalPrefab, 
                                                          level.crystalPortals[_indexs[_index]].transform.position,
                                                          Quaternion.identity,
                                                          crystalContainer);
        }

        #endregion

        #region POWERUP CONTROL

        private void SpawnPowerUp()
        {
            foreach (Transform child in powerUpContainer)
                Destroy(child.gameObject);

            List<int> _indexs = new List<int>();
            for (int i = 0; i < powerUps.Count; i++)
                _indexs.Add(i);

            _indexs.RemoveAt(currentPowerUP);

            int _index = Random.Range(0, _indexs.Count);
            currentPowerUP = _index;

            GameObject _powerUP = Instantiate<GameObject>(powerUps[_indexs[_index]],
                                                          level.playerPortal.transform.position,
                                                          Quaternion.identity,
                                                          powerUpContainer);

            _powerUP.name = powerUps[_indexs[_index]].name;
        }

        private void DestroyAllBullets()
        {
            foreach (Transform _bullet in attackEnemyContainer)
                _bullet.GetComponent<EnemyBullet>().SelfDestroy();
        }

        #endregion
    }
}