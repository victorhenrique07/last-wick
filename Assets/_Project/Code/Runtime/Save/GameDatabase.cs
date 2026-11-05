using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Light;
using Assets._Project.Code.Runtime.Priest;
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
        [SerializeField] private List<AttributeData> attributes;

        [Tooltip("Custo base de um atributo no padre, em pontos de atributo.")]
        [SerializeField, Min(0)] private int priestBaseCost = 5;

        [Tooltip("Incremento de custo para cada nível de atributo no padre.")]
        [SerializeField, Min(0)] private float priestCostStep = 1.8f;

        [Header("Fogueira")]
        [Tooltip("Raio da luz por nível e custo em óleo de cada nível.")]
        [SerializeField] private LightConfig light;

        public CardCollection CardCollection => cardCollection;
        public DeckData StartingDeck => startingDeck;
        public IReadOnlyList<AttributeData> Attributes => attributes;
        public int PriestBaseCost => priestBaseCost;
        public float PriestCostStep => priestCostStep;
        public LightConfig Light => light;

        private void OnValidate()
        {
            if (attributes == null) return;

            foreach (AttributeData attribute in attributes)
            {
                if (attribute == null)
                {
                    Debug.LogWarning("GameDatabase: a lista de atributos tem uma posição vazia.", this);
                    continue;
                }

                if (string.IsNullOrEmpty(attribute.Id))
                    Debug.LogWarning($"AttributeData '{attribute.name}' has no ID set.", attribute);
            }
        }

        public CardData FindCard(string id) => cardCollection.Find(id);

        public AttributeData FindAttribute(string id) => attributes.FirstOrDefault(attr => attr.Id == id);
    }
}
