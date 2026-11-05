using Assets._Project.Code.Runtime.Light;
using Assets._Project.Code.Runtime.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Menu da fogueira: nível de luz, raio atual e do próximo nível, custo em óleo e o botão de compra.
    // Redesenha a cada mudança no save enquanto está aberto (LIGHT-03, LIGHT-16, LIGHT-17).
    public class CampfireMenu : MonoBehaviour
    {
        [SerializeField] private HubPanel panel;

        [Header("Tela")]
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text radiusLabel;
        [SerializeField] private Text costLabel;
        [SerializeField] private Button buyButton;
        [SerializeField] private Color costColor = Color.white;
        [SerializeField] private Color missingOilColor = Color.red; // cor de aviso da paleta (LIGHT-17)

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

        // Ligado ao onClick das velas
        public void Open() => panel.Open();

        // Ligado ao botão "Fechar"
        public void Close() => panel.Close();

        // Ligado ao botão "Alimentar a chama"
        public void Buy() => CampfireShop.TryBuy();

        private void OnOpened()
        {
            SaveSystem.Changed += Redraw;
            Redraw();
        }

        private void OnClosed() => SaveSystem.Changed -= Redraw;

        private void Redraw()
        {
            var config = SaveSystem.Database.Light;
            int level = CampfireShop.Level;
            levelLabel.text = $"Nível {level} de {config.MaxLevel}";

            if (CampfireShop.IsMaxed)
            {
                radiusLabel.text = $"Raio da luz  {LightRules.Radius(config, level):0.0}";
                costLabel.text = "Luz no máximo";
                costLabel.color = costColor;
                buyButton.interactable = false;
                return;
            }

            radiusLabel.text = $"Raio da luz  {LightRules.Radius(config, level):0.0} → {LightRules.Radius(config, level + 1):0.0}";
            costLabel.text = $"Custo: {CampfireShop.NextCost} de óleo";
            costLabel.color = CampfireShop.CanBuy ? costColor : missingOilColor;
            buyButton.interactable = CampfireShop.CanBuy;
        }
    }
}
