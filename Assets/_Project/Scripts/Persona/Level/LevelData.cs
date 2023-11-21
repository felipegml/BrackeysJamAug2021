using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Persona.Level
{
    public class LevelData : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public GameObject playerPortal;
        public List<EnemyPortal> enemyPortals = new List<EnemyPortal>();
        public List<GameObject> crystalPortals = new List<GameObject>();

        [Header("Data")]
        public float cyrstalInterval = 3;

        #endregion
    }
}