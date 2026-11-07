using System.Collections.Generic;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Aba Coleção da mesa de tarot: todas as cartas do jogo, cada uma com nome, ícone e descrição; as bloqueadas
    // trazem embaixo o botão "Desbloquear" e o custo. No topo, "Liberadas: N/M" (TAROT-02 a TAROT-05, TAROT-07,
    // TAROT-08, TAROT-10). O redesenho depois de um desbloqueio vem do SaveSystem.Changed, pelo TarotMenu.
    public class TarotCollectionTab : MonoBehaviour
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private CollectionCardView cardPrefab;
        [SerializeField] private Text unlockedLabel; // "Liberadas: N/M"

        private readonly List<CollectionCardView> views = new();

        public void Refresh()
        {
            IReadOnlyList<CardData> cards = CardLibrary.AllCards;
            ResourceBag owned = SaveSystem.Data.Resources;

            while (views.Count < cards.Count)
                views.Add(Instantiate(cardPrefab, gridContainer));

            int total = 0;
            for (int i = 0; i < views.Count; i++)
            {
                CardData card = i < cards.Count ? cards[i] : null;
                views[i].gameObject.SetActive(card != null);
                if (card == null) continue;

                total++;
                views[i].Show(card, !CardLibrary.IsUnlocked(card), owned, () => CardLibrary.TryUnlock(card));
            }

            unlockedLabel.text = $"Liberadas: {CardLibrary.UnlockedCardCount()}/{total}";
        }
    }
}
