using Assets._Project.Code.Runtime.Run;
using LastWick.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Code.Runtime.UI
{
    // Mostra as estatísticas da run quando o player derrota o último boss.
    public class VictoryPanel : MonoBehaviour
    {
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private string hubSceneName = "Hub";

        [Header("Resumo")]
        [SerializeField] private StatEntryView runTime;
        [SerializeField] private StatEntryView remainingTime;
        [SerializeField] private StatEntryView bossesDefeated;
        [SerializeField] private StatEntryView kills;
        [SerializeField] private StatEntryView finalLevel;

        [Header("Status finais")]
        [SerializeField] private StatEntryView maxHealth;
        [SerializeField] private StatEntryView moveSpeed;
        [SerializeField] private StatEntryView lightRadius;
        [SerializeField] private StatEntryView xpPickupRadius;
        [SerializeField] private StatEntryView dodgeDistance;
        [SerializeField] private StatEntryView dodgeCooldown;
        [SerializeField] private StatEntryView fireRate;
        [SerializeField] private StatEntryView projectileSpeed;
        [SerializeField] private StatEntryView projectileRange;
        [SerializeField] private StatEntryView bonusPierce;

        [Header("Cartas")]
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private CardSlotView cardSlotPrefab;
        [SerializeField] private GameObject noCardsLabel;

        void OnEnable() => GameEvents.RunFinished += RaiseVictoryPanel;
        void OnDisable() => GameEvents.RunFinished -= RaiseVictoryPanel;

        private void RaiseVictoryPanel(RunResult result)
        {
            if (result.Reason != RunEndReason.Victory) return;

            ShowSummary(result);
            ShowStats(result.Stats);
            ShowCards(result);

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        // Ligado ao botão "Voltar ao acampamento"
        public void ReturnToHub()
        {
            Time.timeScale = 1f; // o timeScale é global e continuaria 0 na cena do hub
            SceneManager.LoadScene(hubSceneName);
        }

        private void ShowSummary(RunResult result)
        {
            runTime.SetValue(FormatTime(result.ElapsedTime));
            remainingTime.SetValue(FormatTime(result.RemainingTime));
            bossesDefeated.SetValue(result.MilestonesReached.ToString());
            kills.SetValue(result.Kills.ToString());
            finalLevel.SetValue(result.Level.ToString());
        }

        private void ShowStats(PlayerRunStats stats)
        {
            maxHealth.SetValue($"{stats.MaxHealth:0.#}");
            moveSpeed.SetValue($"{stats.MoveSpeed:0.#}");
            lightRadius.SetValue($"{stats.LightRadius:0.#}");
            xpPickupRadius.SetValue($"{stats.XpPickupRadius:0.#}");
            dodgeDistance.SetValue($"{stats.DodgeDistance:0.#}");
            dodgeCooldown.SetValue($"{stats.DodgeCooldown:0.##}s");
            fireRate.SetValue($"x{stats.FireRateMultiplier:0.##}");
            projectileSpeed.SetValue($"x{stats.ProjectileSpeedMultiplier:0.##}");
            projectileRange.SetValue($"x{stats.ProjectileLifetimeMultiplier:0.##}");
            bonusPierce.SetValue(stats.BonusPierce.ToString());
        }

        private void ShowCards(RunResult result)
        {
            foreach (Transform child in cardsContainer)
                Destroy(child.gameObject);

            noCardsLabel.SetActive(result.Cards.Count == 0);

            foreach (var (card, count) in result.Cards)
                Instantiate(cardSlotPrefab, cardsContainer).Show(card, count);
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            return $"{total / 60}:{total % 60:00}";
        }
    }
}
