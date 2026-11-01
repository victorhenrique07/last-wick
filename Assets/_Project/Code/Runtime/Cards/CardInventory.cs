using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    public class CardInventory : MonoBehaviour
    {
        private readonly Dictionary<CardData, int> cards = new();

        public IReadOnlyDictionary<CardData, int> Cards => cards;

        public event Action Changed;

        private void OnEnable() => GameEvents.CardChosen += Add;
        private void OnDisable() => GameEvents.CardChosen -= Add;

        private void Add(CardData card)
        {
            if (cards.ContainsKey(card))
                cards[card]++;
            else
                cards[card] = 1;

            Changed?.Invoke();
        }

        public float Total(Func<CardData, float> bonus)
        {
            float total = 0f;

            foreach (var (card, count) in cards)
                total += bonus(card) * count;

            return total;
        }

        // Reduções se multiplicam para o resultado nunca ficar negativo.
        public float ReductionMultiplier(Func<CardData, float> reduction)
        {
            float result = 1f;

            foreach (var (card, count) in cards)
                result *= Mathf.Pow(1f - reduction(card), count);

            return result;
        }
    }
}
