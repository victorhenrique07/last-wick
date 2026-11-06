using System;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Cards;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma carta numa lista: desenho, nome e quantidade na tela de vitória;
    // na mesa de tarot também pode aparecer bloqueada e ser clicada.
    public class CardSlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text cardName;
        [SerializeField] private Text count;
        [SerializeField] private Sprite defaultIcon;

        [Header("Mesa de tarot (opcional; vazio na tela de vitória)")]
        [SerializeField] private Button button; // clique da célula
        [SerializeField] private GameObject lockedMark; // texto "Bloqueada"
        [SerializeField] private Color lockedTint = new(0.35f, 0.35f, 0.35f, 1f); // desenho escurecido

        private Color iconColor;

        private void Awake() => iconColor = icon.color;

        // Tela de vitória: carta e quantas vezes foi escolhida
        public void Show(CardData card, int amount)
        {
            ShowCard(card);
            count.text = $"x{amount}";
        }

        // Mesa de tarot: só a carta, sem quantidade
        public void Show(CardData card)
        {
            ShowCard(card);
            if (count != null) count.gameObject.SetActive(false);
        }

        // Escurece o desenho e mostra "Bloqueada" (TAROT-09)
        public void SetLocked(bool locked)
        {
            icon.color = locked ? lockedTint : iconColor;
            if (lockedMark != null) lockedMark.SetActive(locked);
        }

        // O que acontece no clique; nulo deixa a célula sem clique (TAROT-18)
        public void SetOnClick(Action onClick)
        {
            if (button == null) return;

            button.onClick.RemoveAllListeners();
            if (onClick != null) button.onClick.AddListener(() => onClick());
            button.interactable = onClick != null;
        }

        private void ShowCard(CardData card)
        {
            icon.sprite = card.Icon != null ? card.Icon : defaultIcon;
            icon.SetNativeSize();
            cardName.text = card.Name;
        }
    }
}
