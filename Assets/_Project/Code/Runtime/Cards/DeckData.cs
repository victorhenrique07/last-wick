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

        public int MinCards => minCards;
        public int MaxCards => maxCards;

        [SerializeField] private List<CardData> cards = new();

        public IReadOnlyList<CardData> Cards => cards;

        public bool IsValid => cards.Count >= minCards && cards.Count <= maxCards;
    }
}
