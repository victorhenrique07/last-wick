using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Cards;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma carta na lista de cartas da tela de vitória: desenho, nome e quantidade.
    public class CardSlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text cardName;
        [SerializeField] private Text count;
        [SerializeField] private Sprite defaultIcon;

        public void Show(CardData card, int amount)
        {
            icon.sprite = card.Icon != null ? card.Icon : defaultIcon;
            icon.SetNativeSize();
            cardName.text = card.Name;
            count.text = $"x{amount}";
        }
    }
}
