using Assets._Project.Code.Runtime.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.UI
{
    public class LevelUpPanel : MonoBehaviour
    {
        [SerializeField] private GameObject levelUpPanel;
        [SerializeField] private DeckData deck;
        [SerializeField] private CardView[] cards;

        private int pendingChoices;

        void OnEnable() => GameEvents.LevelUp += RaiseLevelUpPanel;
        void OnDisable() => GameEvents.LevelUp -= RaiseLevelUpPanel;

        private void RaiseLevelUpPanel(int level)
        {
            pendingChoices++;

            if (pendingChoices > 1) return;

            ShowOptions();

            levelUpPanel.SetActive(true);

            Time.timeScale = 0f;
        }

        private void Confirm(CardView card)
        {
            GameEvents.RaiseCardChosen(card.Data);

            pendingChoices--;

            if (pendingChoices > 0)
            {
                ShowOptions();
                return;
            }

            levelUpPanel.SetActive(false);

            Time.timeScale = 1f;
        }

        private void ShowOptions()
        {
            var options = new List<CardData>(deck.Cards);

            for (int i = 0; i < cards.Length; i++)
            {
                int index = UnityEngine.Random.Range(0, options.Count);

                cards[i].Show(options[index], Confirm);
                options.RemoveAt(index);
            }
        }
    }
}
