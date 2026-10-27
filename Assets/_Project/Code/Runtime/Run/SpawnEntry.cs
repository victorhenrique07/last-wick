using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    [System.Serializable]
    public class SpawnStage
    {
        public SpawnEntry[] enemies;
        public float spawnInterval = 0.5f;
        public int maxAlive = 50;
    }
}
