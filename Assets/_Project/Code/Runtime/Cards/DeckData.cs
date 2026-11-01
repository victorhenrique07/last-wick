using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    [CreateAssetMenu(fileName = "Deck", menuName = "Last Wick/Deck Data")]
    public class DeckData : ScriptableObject
    {
        [SerializeField, Min(1)] private int minCards = 10;
        [SerializeField, Min(1)] private int maxCards = 15;

        [SerializeField] private List<CardData> cards = new();

        public IReadOnlyList<CardData> Cards => cards;

        public bool IsValid => cards.Count >= minCards && cards.Count <= maxCards;

        public bool TryAdd(CardData card)
        {
            if (cards.Count >= maxCards || cards.Contains(card)) return false;

            cards.Add(card);
            return true;
        }

        public void Remove(CardData card) => cards.Remove(card);
    }
}
