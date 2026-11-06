using System.Collections.Generic;
using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using LastWick.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Menu do ferreiro: ícone, nome e nível da arma equipada, os status que mudam no próximo nível,
    // o custo em osso, ferro e cera e o botão "Forjar". Redesenha a cada mudança no save enquanto está aberto
    // (SMITH-02, SMITH-03, SMITH-04, SMITH-15, SMITH-16).
    public class SmithMenu : MonoBehaviour
    {
        [SerializeField] private HubPanel panel;

        [Header("Arma")]
        [SerializeField] private Image icon;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text levelLabel;

        [Header("Status (uma linha por status que muda)")]
        [SerializeField] private Transform statsContainer;
        [SerializeField] private WeaponStatRowView statRowPrefab;

        [Header("Custo (só o número; o ícone do material fica ao lado)")]
        [SerializeField] private GameObject costRow;
        [SerializeField] private Text bonesLabel;
        [SerializeField] private Text ironLabel;
        [SerializeField] private Text waxLabel;
        [SerializeField] private Color costColor = Color.white;
        [SerializeField] private Color missingColor = new Color32(201, 109, 69, 255); // cor de aviso da paleta (SMITH-16)

        [Header("Máximo e botão")]
        [SerializeField] private GameObject maxedLabel; // "Arma no nível máximo"
        [SerializeField] private Button forgeButton;

        private readonly List<WeaponStatRowView> rows = new();

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

        // Ligado ao onClick da bigorna
        public void Open() => panel.Open();

        // Ligado ao botão "Fechar"
        public void Close() => panel.Close();

        // Ligado ao botão "Forjar": um nível por clique (SMITH-18)
        public void Forge() => SmithShop.TryUpgrade();

        private void OnOpened()
        {
            SaveSystem.Changed += Redraw;
            Redraw();
        }

        private void OnClosed() => SaveSystem.Changed -= Redraw;

        private void Redraw()
        {
            WeaponData weapon = SmithShop.Equipped;
            int level = SmithShop.Level;
            bool maxed = SmithShop.IsMaxed;

            icon.sprite = weapon != null ? weapon.Icon : null;
            icon.enabled = icon.sprite != null;
            nameLabel.text = weapon != null ? weapon.DisplayName : "Sem arma";
            levelLabel.text = weapon != null ? $"Nível {level}/{weapon.MaxLevel}" : "";

            int used = 0;
            if (weapon != null && !maxed)
            {
                WeaponStats current = WeaponStats.Build(weapon, level);
                WeaponStats next = WeaponStats.Build(weapon, level + 1);

                // Só os status que mudam no próximo nível (SMITH-03)
                if (current.Damage != next.Damage)
                    Row(used++).Show("Dano", Number(current.Damage), Number(next.Damage));
                if (current.FireRate != next.FireRate)
                    Row(used++).Show("Cadência", Number(current.FireRate), Number(next.FireRate));
                if (current.ProjectileSpeedMultiplier != next.ProjectileSpeedMultiplier)
                    Row(used++).Show("Velocidade do projétil", Percent(current.ProjectileSpeedMultiplier), Percent(next.ProjectileSpeedMultiplier));
                if (current.LifetimeMultiplier != next.LifetimeMultiplier)
                    Row(used++).Show("Alcance", Percent(current.LifetimeMultiplier), Percent(next.LifetimeMultiplier));
                if (current.BonusPierce != next.BonusPierce)
                    Row(used++).Show("Perfuração", current.BonusPierce.ToString(), next.BonusPierce.ToString());
            }
            for (int i = 0; i < rows.Count; i++)
                rows[i].gameObject.SetActive(i < used);

            costRow.SetActive(weapon != null && !maxed);
            maxedLabel.SetActive(weapon != null && maxed); // SMITH-15
            if (weapon != null && !maxed)
            {
                ResourceBag cost = SmithShop.NextCost;
                ResourceBag owned = SaveSystem.Data.Resources;
                ShowCost(bonesLabel, cost.Bones, owned.Bones);
                ShowCost(ironLabel, cost.Iron, owned.Iron);
                ShowCost(waxLabel, cost.Wax, owned.Wax);
            }

            forgeButton.interactable = SmithShop.CanUpgrade; // desliga sem materiais e no máximo (SMITH-15, SMITH-16)
        }

        // Linha i, criada na primeira vez que é pedida
        private WeaponStatRowView Row(int index)
        {
            while (rows.Count <= index)
                rows.Add(Instantiate(statRowPrefab, statsContainer));
            return rows[index];
        }

        // Material que o nível não pede some; o que falta fica na cor de aviso (SMITH-16)
        private void ShowCost(Text label, int cost, int owned)
        {
            label.transform.parent.gameObject.SetActive(cost > 0);
            label.text = cost.ToString();
            label.color = owned >= cost ? costColor : missingColor;
        }

        private static string Number(float value) => $"{value:0.##}";
        private static string Percent(float multiplier) => $"{multiplier * 100f:0}%";
    }
}
