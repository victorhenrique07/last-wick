using Assets._Project.Code.Runtime.Save;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Code.Runtime.Cards
{
    // As regras de desbloqueio e de deck, separadas da tela. O que muda fica no save, nunca nos assets.
    public static class CardLibrary
    {
        private static DeckData StartingDeck => SaveSystem.Database.StartingDeck;

        // Todas as cartas do jogo, na ordem da coleção (TAROT-07)
        public static IReadOnlyList<CardData> AllCards => SaveSystem.Database.CardCollection.Cards;

        // Carta sem Id não pode ir para o save: nunca é liberada nem entra no deck
        private static bool HasId(CardData card) => card != null && !string.IsNullOrWhiteSpace(card.Id);

        // Liberada = o Id está nas cartas liberadas do save
        public static bool IsUnlocked(CardData card) =>
            HasId(card) && SaveSystem.Data.UnlockedCards.Contains(card.Id);

        // No deck = o Id está no deck do save
        public static bool IsInDeck(CardData card) =>
            HasId(card) && SaveSystem.Data.Deck.Contains(card.Id);

        // As bloqueadas, na ordem da coleção (TAROT-02, TAROT-21)
        public static List<CardData> GetLockedCards() =>
            AllCards.Where(c => HasId(c) && !IsUnlocked(c)).ToList();

        // Para "Liberadas: N/M" (TAROT-10)
        public static int UnlockedCardCount() => AllCards.Count(IsUnlocked);

        // Cobra o custo, libera a carta e grava; já liberada ou sem materiais, não muda nada (TAROT-03, TAROT-04)
        public static bool TryUnlock(CardData card)
        {
            if (!HasId(card) || IsUnlocked(card)) return false;
            if (!SaveSystem.TrySpend(card.UnlockCost)) return false;

            SaveSystem.Data.UnlockedCards.Add(card.Id);
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
            return true;
        }

        // Liberada, fora do deck e com espaço no deck (TAROT-12, TAROT-18, TAROT-20)
        public static bool CanAddToDeck(CardData card) =>
            IsUnlocked(card) && !IsInDeck(card) && DeckCardCount() < StartingDeck.MaxCards;

        public static bool TryAddToDeck(CardData card)
        {
            if (!CanAddToDeck(card)) return false;

            SaveSystem.Data.Deck.Add(card.Id);
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
            return true;
        }

        // Tira a carta do deck e grava (TAROT-13, TAROT-15)
        public static void RemoveFromDeck(CardData card)
        {
            if (!IsInDeck(card)) return;

            SaveSystem.Data.Deck.Remove(card.Id);
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
        }

        public static int DeckCardCount() => SaveSystem.Data.Deck.Count;

        // O deck do save está entre o mínimo e o máximo do deck inicial
        public static bool IsDeckValid() =>
            DeckCardCount() >= StartingDeck.MinCards && DeckCardCount() <= StartingDeck.MaxCards;

        // Os Ids do deck viram cartas; Id que não existe mais é ignorado (TAROT-16)
        public static List<CardData> DeckCards() =>
            SaveSystem.Data.Deck
                .Select(id => SaveSystem.Database.FindCard(id))
                .Where(c => c != null)
                .ToList();
    }
}
