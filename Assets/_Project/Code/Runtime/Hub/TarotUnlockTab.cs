using System.Collections.Generic;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Hub
{
    // Aba Desbloquear da mesa de tarot: uma linha por carta bloqueada, na ordem da coleção,
    // com o custo e o botão "Desbloquear" (TAROT-02, TAROT-05, TAROT-17, TAROT-21).
    public class TarotUnlockTab : MonoBehaviour
    {
        [SerializeField] private Transform rowsContainer;
        [SerializeField] private UnlockRowView rowPrefab;
        [SerializeField] private GameObject allUnlockedLabel; // "Todas as cartas foram liberadas."

        // Linhas já criadas; as que sobram ficam escondidas
        private readonly List<UnlockRowView> rows = new();

        public void Refresh()
        {
            List<CardData> locked = CardLibrary.GetLockedCards();
            ResourceBag owned = SaveSystem.Data.Resources;

            while (rows.Count < locked.Count)
                rows.Add(Instantiate(rowPrefab, rowsContainer));

            for (int i = 0; i < rows.Count; i++)
            {
                bool used = i < locked.Count;
                rows[i].gameObject.SetActive(used);
                if (!used) continue;

                // A carta liberada some da lista no redesenho que o save dispara (TAROT-05)
                CardData card = locked[i];
                rows[i].Show(card, owned, () => CardLibrary.TryUnlock(card));
            }

            allUnlockedLabel.SetActive(locked.Count == 0);
        }
    }
}
