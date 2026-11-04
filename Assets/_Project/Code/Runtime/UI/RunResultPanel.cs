using Assets._Project.Code.Runtime.Run;
using LastWick.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Core;

namespace Assets._Project.Code.Runtime.UI
{
    // Mostra as estatísticas da run quando ela termina, em vitória ou em morte.
    // As informações são as mesmas; só o título muda.
    public class RunResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private string hubSceneName = "Hub";

        [Header("Título")]
        [SerializeField] private Text title;
        [SerializeField] private string victoryTitle = "A CHAMA RESISTE";
        [SerializeField] private string deathTitle = "A CHAMA SE APAGOU";
        [SerializeField] private string darknessTitle = "A ESCURIDÃO VENCEU";

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

        [Header("Recompensas")]
        [SerializeField] private RewardEntryView gold;
        [SerializeField] private RewardEntryView oil;
        [SerializeField] private RewardEntryView bones;
        [SerializeField] private RewardEntryView iron;
        [SerializeField] private RewardEntryView wax;
        [SerializeField] private GameObject lossLabel; // "Metade do que você coletou...", só na morte e na escuridão

        void OnEnable() => GameEvents.RunFinished += RaiseResultPanel;
        void OnDisable() => GameEvents.RunFinished -= RaiseResultPanel;

        private void RaiseResultPanel(RunResult result)
        {
            if (result.Reason == RunEndReason.Quit) return;

            title.text = result.Reason switch
            {
                RunEndReason.Victory => victoryTitle,
                RunEndReason.Darkness => darknessTitle,
                _ => deathTitle,
            };
            ShowSummary(result);
            ShowStats(result.Stats);
            ShowCards(result);

            panel.SetActive(true);
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
