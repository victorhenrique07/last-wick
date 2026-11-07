using Assets._Project.Code.Runtime.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Menu da mesa de tarot: só as abas Coleção e Deck (TAROT-09). Sempre abre na Coleção
    // e redesenha a aba aberta a cada mudança no save enquanto está aberto (TAROT-01, TAROT-06).
    public class TarotMenu : MonoBehaviour
    {
        public const int CollectionTabIndex = 0;
        public const int DeckTabIndex = 1;

        [SerializeField] private HubPanel panel;

        [Header("Abas (na ordem dos índices)")]
        [SerializeField] private TarotCollectionTab collectionTab;
        [SerializeField] private TarotDeckTab deckTab;

        [Header("Botões das abas")]
        [SerializeField] private Text[] tabLabels; // Coleção, Deck
        [SerializeField] private Color selectedTabColor = Color.white;
        [SerializeField] private Color idleTabColor = Color.gray;

        private int current;

        void OnEnable()
        {
            panel.Opened += OnOpened;
            panel.Closed += OnClosed;
        }

        void OnDisable()
        {
            panel.Opened -= OnOpened;
            panel.Closed -= OnClosed;
            SaveSystem.Changed -= Redraw;
        }

        // Ligado ao onClick da mesa de tarot
        public void Open() => panel.Open();

        // Ligado ao botão "Fechar"
        public void Close() => panel.Close();

        // Ligado aos botões das abas
        public void ShowTab(int index)
        {
            current = index;
            collectionTab.gameObject.SetActive(index == CollectionTabIndex);
            deckTab.gameObject.SetActive(index == DeckTabIndex);

            for (int i = 0; i < tabLabels.Length; i++)
                tabLabels[i].color = i == index ? selectedTabColor : idleTabColor;

            Redraw();
        }

        private void OnOpened()
        {
            SaveSystem.Changed += Redraw;
            ShowTab(CollectionTabIndex); // TAROT-01
        }

        private void OnClosed() => SaveSystem.Changed -= Redraw;

        private void Redraw()
        {
            switch (current)
            {
                case CollectionTabIndex: collectionTab.Refresh(); break;
                case DeckTabIndex: deckTab.Refresh(); break;
            }
        }
    }
}
