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

            if (!ShowOptions()) return;

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

        // Devolve falso quando não há carta nenhuma para oferecer; aí o level up não abre
        private bool ShowOptions()
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
                    if (card != null && !string.IsNullOrWhiteSpace(card.Id) && !options.Contains(card)) options.Add(card);
                }
            }

            if (options.Count == 0)
            {
                Debug.LogError("LevelUpPanel: nem o deck salvo nem o deck inicial têm cartas; o level up foi pulado.");
                pendingChoices = 0;
                return false;
            }

            // Se nem o deck inicial completar, mostra só as opções que houver e esconde as outras
            for (int i = 0; i < cards.Length; i++)
            {
                bool hasOption = options.Count > 0;
                cards[i].gameObject.SetActive(hasOption);
                if (!hasOption) continue;

                int index = UnityEngine.Random.Range(0, options.Count);

                cards[i].Show(options[index], Confirm);
                options.RemoveAt(index);
            }

            return true;
        }
    }
}
