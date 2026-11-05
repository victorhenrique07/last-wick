using System.Collections.Generic;
using System.Text;
using Assets._Project.Code.Runtime.Hub;
using Assets._Project.Code.Runtime.Player;
using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using LastWick.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Priest
{
    // Menu do padre: nível do personagem, custo do próximo ponto e uma linha por atributo do GameDatabase.
    // Redesenha a cada mudança no save enquanto está aberto (PRST-02, PRST-03, PRST-08).
    public class PriestMenu : MonoBehaviour
    {
        [SerializeField] private HubPanel panel;
        [SerializeField] private PlayerStats baseStats;

        [Header("Tela")]
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text costLabel;
        [SerializeField] private Transform rowsContainer;
        [SerializeField] private AttributeRowView rowPrefab;
        [SerializeField] private Color costColor = Color.white;
        [SerializeField] private Color missingGoldColor = Color.red; // PRST-24

        private readonly List<(AttributeData attribute, AttributeRowView row)> rows = new();

        void OnEnable()
        {
            panel.Opened += OnOpened;
            panel.Closed += OnClosed;
        }

        void OnDisable()
        {
            panel.Opened -= OnOpened;
            panel.Closed -= OnClosed;
            SaveSystem.Changed -= Redraw;
        }

        // Ligado ao onClick da igreja
        public void Open() => panel.Open();

        // Ligado ao botão "Fechar"
        public void Close() => panel.Close();

        private void OnOpened()
        {
            if (rows.Count == 0) CreateRows();
            SaveSystem.Changed += Redraw;
            Redraw();
        }

        private void OnClosed() => SaveSystem.Changed -= Redraw;

        // Uma linha por atributo da lista: um atributo novo aparece sem código (PRST-21)
        private void CreateRows()
        {
            foreach (var attribute in SaveSystem.Database.Attributes)
            {
                if (attribute == null || string.IsNullOrEmpty(attribute.Id)) continue;
                rows.Add((attribute, Instantiate(rowPrefab, rowsContainer)));
            }
        }

        private void Redraw()
        {
            bool canBuy = PriestShop.CanBuy();
            levelLabel.text = $"Nível {PriestShop.CharacterLevel()}";
            costLabel.text = $"Próximo ponto: {PriestShop.NextCost()} de ouro";
            costLabel.color = canBuy ? costColor : missingGoldColor;

            var attributes = SaveSystem.Database.Attributes;
            var current = PermanentStats.Build(baseStats, attributes, PriestShop.LevelOf);

            foreach (var (attribute, row) in rows)
            {
                // O "próximo" usa a mesma conta da run, com um ponto a mais neste atributo
                var next = PermanentStats.Build(baseStats, attributes, a => PriestShop.LevelOf(a) + (a == attribute ? 1 : 0));
                var buy = attribute;
                row.Show(attribute.Icon, attribute.DisplayName, PriestShop.LevelOf(attribute),
                    Effects(attribute, current, next), canBuy, () => PriestShop.TryBuy(buy));
            }
        }

        private static string Effects(AttributeData attribute, StatBlock current, StatBlock next)
        {
            var text = new StringBuilder();
            foreach (var effect in attribute.Effects)
            {
                if (text.Length > 0) text.Append('\n');
                text.Append(StatLabels.Name(effect.Stat)).Append("  ")
                    .Append(StatLabels.Format(effect.Stat, PermanentStats.Get(current, effect.Stat)))
                    .Append(" → ")
                    .Append(StatLabels.Format(effect.Stat, PermanentStats.Get(next, effect.Stat)));
            }
            return text.ToString();
        }
    }
}
