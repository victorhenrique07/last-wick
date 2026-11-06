using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Aba Deck da mesa de tarot: "No deck" e "Disponíveis"; um clique move a carta de uma grade
    // para a outra e a CardLibrary grava o save (TAROT-11 a TAROT-15, TAROT-18, TAROT-19).
    public class TarotDeckTab : MonoBehaviour
    {
        [SerializeField] private CardSlotView slotPrefab;
        [SerializeField] private Transform deckContainer; // "No deck"
        [SerializeField] private Transform availableContainer; // "Disponíveis"
        [SerializeField] private Text countLabel; // "Deck: N/15"
        [SerializeField] private Text minimumWarning; // "O deck precisa de pelo menos 10 cartas."

        private readonly List<CardSlotView> deckSlots = new();
        private readonly List<CardSlotView> availableSlots = new();

        public void Refresh()
        {
            DeckData rules = SaveSystem.Database.StartingDeck;
            List<CardData> inDeck = CardLibrary.DeckCards();
            List<CardData> available = CardLibrary.AllCards
                .Where(c => c != null && CardLibrary.IsUnlocked(c) && !CardLibrary.IsInDeck(c))
                .ToList();

            Fill(deckSlots, deckContainer, inDeck, card => () => CardLibrary.RemoveFromDeck(card));

            // Com o deck cheio, as disponíveis ficam sem clique (TAROT-18)
            Fill(availableSlots, availableContainer, available,
                card => CardLibrary.CanAddToDeck(card) ? () => CardLibrary.TryAddToDeck(card) : null);

            int count = CardLibrary.DeckCardCount();
            countLabel.text = $"Deck: {count}/{rules.MaxCards}";
            minimumWarning.text = $"O deck precisa de pelo menos {rules.MinCards} cartas.";
            minimumWarning.gameObject.SetActive(count < rules.MinCards);
        }

        // Uma célula por carta; as que sobram ficam escondidas
        private void Fill(List<CardSlotView> slots, Transform container, List<CardData> cards, Func<CardData, Action> click)
        {
            while (slots.Count < cards.Count)
                slots.Add(Instantiate(slotPrefab, container));

            for (int i = 0; i < slots.Count; i++)
            {
                bool used = i < cards.Count;
                slots[i].gameObject.SetActive(used);
                if (!used) continue;

                slots[i].Show(cards[i]);
                slots[i].SetLocked(false);
                slots[i].SetOnClick(click(cards[i]));
            }
        }
    }
}
