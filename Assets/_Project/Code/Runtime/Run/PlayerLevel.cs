using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    public class PlayerLevel : MonoBehaviour
    {
        [SerializeField] private RunConfig config;

        private int xp;         // XP acumulado dentro do nível atual
        private int level = 0;  // nível atual da run

        public int Level => level;
        public float Progress => level + xp / (float)ExpToNextLevel();

        private int ExpToNextLevel() => config.BaseExp + config.ExpGrowth * level;

        private void OnEnable()
        {
            GameEvents.XpGained += AddExp;
        }

        private void OnDisable() => GameEvents.XpGained -= AddExp;

        private void AddExp(int amountExp)
        {
            xp += amountExp;

            while (xp >= ExpToNextLevel())
            {
                xp -= ExpToNextLevel();
                level++;

                GameEvents.RaiseLevelUp(level);
                Debug.Log($"Subiu para o nível {level}");
            }
        }
    }
}
