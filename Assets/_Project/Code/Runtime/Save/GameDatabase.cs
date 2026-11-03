using Assets._Project.Code.Runtime.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Save
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Last Wick/Game Database")]
    public class GameDatabase : ScriptableObject
    {
        [SerializeField] private CardCollection cardCollection;
        [SerializeField] private DeckData startingDeck;

        public CardCollection CardCollection => cardCollection;
        public DeckData StartingDeck => startingDeck;

        public CardData FindCard(string id) => cardCollection.Find(id);
    }
}
