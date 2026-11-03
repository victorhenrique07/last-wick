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

        private void OnEnable()
        {
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.BossDefeated += OnBossDefeated;
            GameEvents.OilCollected += OnOilCollected;
            GameEvents.RunFinished += OnRunFinished;
        }

        private void OnDisable()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.BossDefeated -= OnBossDefeated;
            GameEvents.OilCollected -= OnOilCollected;
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

            BossReward boss = config.Bosses[bossIndex];

            collected.Gold += boss.Gold;
            collected.Oil += boss.Oil;
        }

        private void OnOilCollected(int amount)
        {
            if (finished)
                return;

            collected.Oil += amount;
        }

        private void OnRunFinished(RunResult result)
        {
            finished = true;
        }
    }
}
