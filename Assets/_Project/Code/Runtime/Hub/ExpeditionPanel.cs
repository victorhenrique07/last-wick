using Assets._Project.Code.Runtime.Cards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Hub
{
    // Painel "Partir para a expedição?", aberto pelo portão do hub: mostra o mapa e o deck salvo,
    // e só deixa partir com um deck de tamanho válido (HUB-03 a HUB-07, HUB-13, HUB-15).
    public class ExpeditionPanel : MonoBehaviour
    {
        [SerializeField] private HubPanel panel;
        [SerializeField] private string mapName = "Cemitério - Entrada";
        [SerializeField] private string runSceneName = "FirstOne";

        [Header("Tela")]
        [SerializeField] private Text mapLabel;
        [SerializeField] private Text deckLabel;
        [SerializeField] private GameObject invalidDeckLabel; // "Monte um deck de 10 a 15 cartas na mesa de tarot."
        [SerializeField] private Button departButton;

        private bool loading;

        // Ligado ao onClick do portão
        public void Open()
        {
            int count = CardLibrary.DeckCardCount();
            bool valid = CardLibrary.IsDeckValid(); // a mesma regra da mesa de tarot

            mapLabel.text = mapName;
            deckLabel.text = $"Deck: {count} cartas";
            invalidDeckLabel.SetActive(!valid);
            departButton.interactable = valid;

            panel.Open();
        }

        // Ligado ao botão "Partir"
        public void Depart()
        {
            if (loading) return; // vários cliques carregam a run uma vez só

            if (!Application.CanStreamedLevelBeLoaded(runSceneName))
            {
                Debug.LogError($"ExpeditionPanel: a cena '{runSceneName}' não está no build.", this);
                return;
            }

            loading = true;
            Time.timeScale = 1f; // a run sempre começa andando, venha de onde vier
            SceneManager.LoadScene(runSceneName);
        }

        // Ligado ao botão "Ficar"
        public void Stay() => panel.Close();
    }
}
