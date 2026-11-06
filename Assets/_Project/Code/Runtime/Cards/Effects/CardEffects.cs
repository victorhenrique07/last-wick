using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    // Cria a mecânica de uma carta na primeira cópia e avisa a mesma instância nas cópias seguintes.
    // Ouve o CardAdded do CardInventory, e não o GameEvents.CardChosen: assim a contagem de cópias já está feita.
    [RequireComponent(typeof(CardInventory))]
    public class CardEffects : MonoBehaviour
    {
        private readonly Dictionary<CardData, CardEffect> effects = new();
        private CardInventory inventory;

        private void Awake() => inventory = GetComponent<CardInventory>();

        private void OnEnable() => inventory.CardAdded += OnCardAdded;
        private void OnDisable() => inventory.CardAdded -= OnCardAdded;

        private void OnCardAdded(CardData card, int copies)
        {
            // Carta só de números: os bônus já entram pelo PlayerRunStats (CEFX-05)
            if (card.EffectPrefab == null) return;

            // Cópia nova de uma carta que já tem efeito: a mecânica decide o que muda (CEFX-03)
            if (effects.TryGetValue(card, out CardEffect existing))
            {
                existing.SetCopies(copies);
                return;
            }

            GameObject instance = Instantiate(card.EffectPrefab, transform);
            CardEffect effect = instance.GetComponent<CardEffect>();

            // CEFX-10: sem a mecânica, a carta vale só pelos números
            if (effect == null)
            {
                Debug.LogWarning($"CardEffects: o prefab de efeito da carta '{card.Id}' não tem CardEffect; a carta vale só pelos números.", this);
                Destroy(instance);
                return;
            }

            // CEFX-02: primeira cópia
            effect.Attach(gameObject, card);
            effect.SetCopies(copies);
            effects[card] = effect;
        }
    }
}
