using Assets._Project.Code.Runtime.Run;
using LastWick.Player;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Mostra as estatísticas da run quando o player derrota o último boss.
    public class VictoryPanel : MonoBehaviour
    {
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Text summaryText;
        [SerializeField] private Text statsText;
        [SerializeField] private Text cardsText;

        void OnEnable() => GameEvents.RunFinished += RaiseVictoryPanel;
        void OnDisable() => GameEvents.RunFinished -= RaiseVictoryPanel;

        private void RaiseVictoryPanel(RunResult result)
        {
            if (result.Reason != RunEndReason.Victory) return;

            summaryText.text = FormatSummary(result);
            statsText.text = FormatStats(result.Stats);
            cardsText.text = FormatCards(result);

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private static string FormatSummary(RunResult result)
        {
            var text = new StringBuilder();

            text.AppendLine($"Tempo da run: {FormatTime(result.ElapsedTime)}");
            text.AppendLine($"Tempo restante: {FormatTime(result.RemainingTime)}");
            text.AppendLine($"Bosses derrotados: {result.MilestonesReached}");
            text.AppendLine($"Inimigos mortos: {result.Kills}");
            text.Append($"Nível final: {result.Level}");

            return text.ToString();
        }

        private static string FormatStats(PlayerRunStats stats)
        {
            var text = new StringBuilder("Status finais\n");

            text.AppendLine($"Vida máxima: {stats.MaxHealth:0.#}");
            text.AppendLine($"Velocidade: {stats.MoveSpeed:0.#}");
            text.AppendLine($"Raio da luz: {stats.LightRadius:0.#}");
            text.AppendLine($"Coleta de XP: {stats.XpPickupRadius:0.#}");
            text.AppendLine($"Distância da esquiva: {stats.DodgeDistance:0.#}");
            text.AppendLine($"Recarga da esquiva: {stats.DodgeCooldown:0.##}s");
            text.AppendLine($"Cadência: x{stats.FireRateMultiplier:0.##}");
            text.AppendLine($"Velocidade do projétil: x{stats.ProjectileSpeedMultiplier:0.##}");
            text.AppendLine($"Alcance do projétil: x{stats.ProjectileLifetimeMultiplier:0.##}");
            text.Append($"Perfuração extra: {stats.BonusPierce}");

            return text.ToString();
        }

        private static string FormatCards(RunResult result)
        {
            var text = new StringBuilder("Cartas\n");

            if (result.Cards.Count == 0)
            {
                text.Append("Nenhuma");
                return text.ToString();
            }

            foreach (var (card, count) in result.Cards)
                text.AppendLine($"{card.Name} x{count}");

            return text.ToString();
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            return $"{total / 60}:{total % 60:00}";
        }
    }
}
