using Persona.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Gameplay.Data
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Level")]
    public class Level_SO : ScriptableObject
    {
        [Header("Data")]
        public GameObject levelGrid;
        public LevelData levelData;
        public float levelTime = 0;
    }
}