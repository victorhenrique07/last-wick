using Assets._Project.Code.Runtime.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Assets._Project.Code.Runtime.Core;
using Assets._Project.Code.Runtime.Save;

namespace Assets._Project.Code.Runtime.UI
{
    public class LevelUpPanel : MonoBehaviour
    {
        [SerializeField] private GameObject levelUpPanel;
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
            // As opções saem do deck montado na mesa de tarot (TAROT-16)
            var options = CardLibrary.DeckCards();

            // Save com deck pequeno (Play direto na cena): completa com o deck inicial para a run não travar
            if (options.Count < cards.Length)
            {
                Debug.LogWarning($"LevelUpPanel: o deck salvo tem {options.Count} cartas; completando com o deck inicial.");
                foreach (CardData card in SaveSystem.Database.StartingDeck.Cards)
                {
                    if (options.Count >= cards.Length) break;
                    if (card != null && !options.Contains(card)) options.Add(card);
                }
            }

            for (int i = 0; i < cards.Length; i++)
            {
                int index = UnityEngine.Random.Range(0, options.Count);

                cards[i].Show(options[index], Confirm);
                options.RemoveAt(index);
            }
        }
    }
}
