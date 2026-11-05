using Assets._Project.Code.Runtime.Bosses;
using Assets._Project.Code.Runtime.Core;
using Assets._Project.Code.Runtime.Save;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    public class RunRewards : MonoBehaviour
    {
        [SerializeField] private RewardConfig config;

        private ResourceBag collected;

        public ResourceBag Collected => collected;

        private bool finished;

        private System.Random rng = new System.Random();

        // Sem config não há o que somar: desliga o componente antes do OnEnable, e a run termina com 0 de tudo
        private void Awake()
        {
            if (config != null) return;

            Debug.LogError("RunRewards: sem RewardConfig; a run não vai render recompensas.", this);
            enabled = false;
        }

        private void OnEnable()
        {
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.BossDefeated += OnBossDefeated;
            GameEvents.RunFinished += OnRunFinished;
        }

        private void OnDisable()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.BossDefeated -= OnBossDefeated;
            GameEvents.RunFinished -= OnRunFinished;
        }

        private void OnEnemyKilled(int points)
        {
            var chance = rng.NextDouble();

            if (chance < config.MaterialChance && !finished)
            {
                var materials = RewardRules.RandomMaterials(1, rng);
                collected += materials;
            }
        }

        private void OnBossDefeated(int bossIndex)
        {
            if (finished)
                return;

            if (bossIndex < 0 || bossIndex >= config.Bosses.Length)
            {
                Debug.LogError($"RunRewards: o boss {bossIndex} não tem linha no RewardConfig; ele não rende recompensa.", this);
                return;
            }

            BossReward boss = config.Bosses[bossIndex];

            collected.Gold += boss.Gold;
            collected.Oil += boss.Oil;
        }

        private void OnRunFinished(RunResult result)
        {
            finished = true;
        }
    }
}
