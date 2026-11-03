using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    [CreateAssetMenu(fileName = "CardCollection", menuName = "Last Wick/Card Collection")]
    public class CardCollection : ScriptableObject
    {
        [SerializeField] private List<CardData> cards = new();

        public IReadOnlyList<CardData> Cards => cards;

        public CardData Find(string id) => cards.Where(c => c != null && c.Id == id).FirstOrDefault();

        private void OnValidate()
        {
            if (cards == null) return;

            var ids = new HashSet<string>();

            foreach (CardData card in cards)
            {
                if (card == null) continue;

                if (string.IsNullOrWhiteSpace(card.Id))
                    Debug.LogWarning($"CardCollection: a carta {card.name} está sem Id.", this);
                else if (!ids.Add(card.Id))
                    Debug.LogWarning($"CardCollection: o Id \"{card.Id}\" se repete em {card.name}.", this);
            }
        }
    }
}
