using Assets._Project.Code.Runtime.Cards;
using LastWick.Player;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    // Encerra a run em vitória quando o último boss morre e entrega o resultado para a tela.
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

        private void OnEnable() => GameEvents.BossDefeated += OnBossDefeated;
        private void OnDisable() => GameEvents.BossDefeated -= OnBossDefeated;

        private void OnBossDefeated(int bossIndex)
        {
            if (bossIndex != config.Milestones.Length - 1) return;

            GameEvents.RaiseRunFinished(new RunResult
            {
                Reason = RunEndReason.Victory,
                ElapsedTime = timer.Elapsed,
                RemainingTime = timer.Remaining,
                Points = progress.Points,
                Kills = progress.Kills,
                MilestonesReached = progress.MilestonesReached,
                Level = playerLevel.Level,
                Cards = inventory.Cards,
                Stats = stats
            });
        }
    }
}
