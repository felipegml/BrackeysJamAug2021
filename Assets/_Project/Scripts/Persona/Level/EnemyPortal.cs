using Persona.Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Level
{
    public class EnemyPortal : MonoBehaviour
    {
        #region VARIABLES

        [Header("Data")]
        public float spawnTime = 3;
        public List<EnemyControl> enemys = new List<EnemyControl>();
        public List<LevelPath> paths = new List<LevelPath>();

        //Private
        private bool paused = true;
        private float currentSpawnTime;

        //Events
        public static event CustomEvent SpawnEnemy_Event;

        #endregion

        #region UNITY EVENTS

        void Update()
        {
            if(!paused)
            {
                if(currentSpawnTime < spawnTime)
                    currentSpawnTime += Time.deltaTime;
                else
                    TriggerEvent();
            }
        }

        #endregion

        #region SETUP

        public void Setup()
        {
            paused = false;
            TriggerEvent();
        }

        private void TriggerEvent()
        {
            currentSpawnTime = 0;
            SpawnEnemy_Event?.Invoke(new object[] { this });
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        #endregion

        #region FUNCTIONS

        //Path
        public int GetPathIndex()
        {
            return 0;
        }

        public int GetPathNextIndex(int _path, int _point)
        {
            if (_point >= paths[_path].points.Count - 1)
                return 0;
            else
                return _point+1;
        }

        public Vector2 GetNextPoint(int _path, int _point)
        {
            Vector2 _pos = new Vector2(paths[_path].points[_point].transform.position.x,
                                       paths[_path].points[_point].transform.position.y);
            return _pos;
        }
        
        public float GetPathTime(int _path, int _point, float _speed)
        {
            Vector2 _posA = GetNextPoint(_path, _point);
            Vector2 _posB = GetNextPoint(_path, GetPathNextIndex(_path, _point));

            float _time = Vector2.Distance(_posA, _posB) / _speed;

            return Mathf.Abs(_time);
        }

        //Enemy
        public EnemyControl GetEnemyPrefab()
        {
            return enemys.First();
        }

        #endregion
    }
}