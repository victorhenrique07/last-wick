using Assets._Project.Code.Runtime.Cards;
using LastWick.Player;
using UnityEngine;
using Assets._Project.Code.Runtime.Core;

namespace Assets._Project.Code.Runtime.Run
{
    // Encerra a run quando o último boss morre (vitória) ou quando o player morre,
    // e entrega o resultado para a tela de resultado.
    public class RunFinisher : MonoBehaviour
    {
        [SerializeField] private RunConfig config;
        [SerializeField] private RunTimer timer;
        [SerializeField] private RunProgress progress;
        [SerializeField] private PlayerLevel playerLevel;

        private CardInventory inventory;
        private PlayerRunStats stats;

        void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            inventory = player.GetComponent<CardInventory>();
            stats = player.GetComponent<PlayerRunStats>();
        }

        private void OnEnable()
        {
            GameEvents.BossDefeated += OnBossDefeated;
            GameEvents.PlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.BossDefeated -= OnBossDefeated;
            GameEvents.PlayerDied -= OnPlayerDied;
        }

        private void OnBossDefeated(int bossIndex)
        {
            if (bossIndex != config.Milestones.Length - 1) return;

            GameEvents.RaiseRunFinished(BuildResult(RunEndReason.Victory));
        }

        // A escuridão dos 10:00 também mata o player; o relógio zerado diz qual foi o caso
        private void OnPlayerDied()
        {
            var reason = timer.Remaining <= 0f ? RunEndReason.Darkness : RunEndReason.Death;
            GameEvents.RaiseRunFinished(BuildResult(reason));
        }

        private RunResult BuildResult(RunEndReason reason) => new RunResult
        {
            Reason = reason,
            ElapsedTime = timer.Elapsed,
            RemainingTime = timer.Remaining,
            Points = progress.Points,
            Kills = progress.Kills,
            MilestonesReached = progress.MilestonesReached,
            Level = playerLevel.Level,
            Cards = inventory.Cards,
            Stats = stats
        };
    }
}
