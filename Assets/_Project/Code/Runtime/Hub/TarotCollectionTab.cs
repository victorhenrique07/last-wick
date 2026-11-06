using System.Collections.Generic;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Aba Coleção da mesa de tarot: todas as cartas do jogo, as bloqueadas escurecidas com "Bloqueada",
    // o detalhe da carta selecionada e "Liberadas: N/M" (TAROT-07 a TAROT-10).
    public class TarotCollectionTab : MonoBehaviour
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private CardSlotView slotPrefab;
        [SerializeField] private CardDetailView detail;
        [SerializeField] private Text unlockedLabel; // "Liberadas: N/M"

        private readonly List<CardSlotView> slots = new();
        private CardData selected;

        // Abrir a aba volta para a primeira carta
        void OnEnable() => selected = null;

        public void Refresh()
        {
            IReadOnlyList<CardData> cards = CardLibrary.AllCards;

            int total = 0;
            while (slots.Count < cards.Count)
                slots.Add(Instantiate(slotPrefab, gridContainer));

            for (int i = 0; i < slots.Count; i++)
            {
                CardData card = i < cards.Count ? cards[i] : null;
                slots[i].gameObject.SetActive(card != null);
                if (card == null) continue;

                total++;
                slots[i].Show(card);
                slots[i].SetLocked(!CardLibrary.IsUnlocked(card));
                slots[i].SetOnClick(() => Select(card));
                if (selected == null) selected = card;
            }

            unlockedLabel.text = $"Liberadas: {CardLibrary.UnlockedCardCount()}/{total}";
            if (selected != null) detail.Show(selected, !CardLibrary.IsUnlocked(selected));
        }

        private void Select(CardData card)
        {
            selected = card;
            detail.Show(card, !CardLibrary.IsUnlocked(card));
        }
    }
}
