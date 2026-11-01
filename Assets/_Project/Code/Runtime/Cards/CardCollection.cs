using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    [CreateAssetMenu(fileName = "CardCollection", menuName = "Last Wick/Card Collection")]
    public class CardCollection : ScriptableObject
    {
        [SerializeField] private List<CardData> cards = new();

        public IReadOnlyList<CardData> Cards => cards;
    }
}
