using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Core;
using Assets._Project.Code.Runtime.Save;
using LastWick.Combat;
using LastWick.Player;
using UnityEngine;

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
        [SerializeField] private RunRewards rewards;

        private bool finished;
        private bool victoryPending; // o boss final morreu; a run termina no LateUpdate deste frame

        private CardInventory inventory;
        private PlayerRunStats stats;

        void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            inventory = player.GetComponent<CardInventory>();
            stats = player.GetComponent<PlayerRunStats>();

            if (rewards == null)
                Debug.LogError("RunFinisher: sem RunRewards; a run vai entregar 0 de tudo.", this);
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

            // Só anota: o RunRewards também ouve o BossDefeated e pode ainda não ter somado o boss final
            victoryPending = true;
        }

        // Roda depois de todos os ouvintes do frame, então o Collected já inclui o boss final
        void LateUpdate()
        {
            if (!victoryPending) return;

            Finish(RunEndReason.Victory);
        }

        // A escuridão dos 10:00 também mata o player; o relógio zerado diz qual foi o caso
        private void OnPlayerDied()
        {
            // O boss final morreu antes, neste mesmo frame: vale a vitória
            if (victoryPending) return;

            var reason = timer.Remaining <= 0f ? RunEndReason.Darkness : RunEndReason.Death;

            Finish(reason);
        }

        private RunResult BuildResult(RunEndReason reason)
        {
            // Sem RunRewards, entrega 0 em vez de travar o fim da run (a tela de resultado ainda abre)
            var collected = rewards != null ? rewards.Collected : default;

            return new RunResult
            {
                Reason = reason,
                ElapsedTime = timer.Elapsed,
                RemainingTime = timer.Remaining,
                Points = progress.Points,
                Kills = progress.Kills,
                MilestonesReached = progress.MilestonesReached,
                Level = playerLevel.Level,
                Cards = inventory.Cards,
                Stats = stats,
                Collected = collected,
                Delivered = RewardRules.Deliver(collected, reason)
            };
        }

        private void Finish(RunEndReason reason)
        {
            if (finished) return;

            finished = true;

            var result = BuildResult(reason);

            SaveSystem.Add(result.Delivered);
            SaveSystem.Save();

            GameEvents.RaiseRunFinished(result);
        }
    }
}
