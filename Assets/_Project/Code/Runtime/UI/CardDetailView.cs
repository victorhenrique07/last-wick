using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;

namespace Assets._Project.Code.Runtime.UI
{
    // O detalhe da carta selecionada na Coleção: nome, ícone, descrição, uma linha por efeito
    // e, se bloqueada, "Bloqueada" com o custo (TAROT-08, TAROT-09).
    public class CardDetailView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite defaultIcon; // carta sem desenho
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text descriptionLabel;
        [SerializeField] private Text effectsLabel; // uma linha por bônus

        [Header("Bloqueada")]
        [SerializeField] private GameObject lockedGroup; // texto "Bloqueada" e o custo, escondidos se liberada
        [SerializeField] private Text costLabel;

        public void Show(CardData card, bool locked)
        {
            icon.sprite = card.Icon != null ? card.Icon : defaultIcon;
            nameLabel.text = card.Name;
            descriptionLabel.text = card.Description;
            effectsLabel.text = string.Join("\n", CardDescriber.Lines(card));

            lockedGroup.SetActive(locked);
            if (locked) costLabel.text = CostText(card.UnlockCost);
        }

        private static string CostText(ResourceBag cost) =>
            $"Custo: {cost.Bones} osso  {cost.Iron} ferro  {cost.Wax} cera";
    }
}
