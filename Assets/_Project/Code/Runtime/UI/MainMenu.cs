using Assets._Project.Code.Runtime.Save;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets._Project.Code.Runtime.UI
{
    // Menu principal: novo jogo, continuar, opções (ainda sem tela) e sair (AD-013).
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string hubSceneName = "Hub";
        [SerializeField] private MenuOption continueOption;
        [SerializeField] private MenuOption optionsOption;
        [SerializeField] private GameObject confirmNewGame; // painel "Começar de novo?"

        void Start()
        {
            continueOption.SetInteractable(SaveSystem.HasSave);
            optionsOption.SetInteractable(false); // até a tela de opções existir
            confirmNewGame.SetActive(false);
        }

        void Update()
        {
            if (confirmNewGame.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                CancelNewGame();
        }

        // Ligado ao "Novo jogo": com save, pede confirmação antes de apagar o progresso
        public void NewGame()
        {
            if (SaveSystem.HasSave)
                confirmNewGame.SetActive(true);
            else
                StartNewGame();
        }

        // Ligado ao "Confirmar" do painel
        public void StartNewGame()
        {
            SaveSystem.StartNew();
            SceneManager.LoadScene(hubSceneName);
        }

        // Ligado ao "Cancelar" do painel e ao Esc
        public void CancelNewGame() => confirmNewGame.SetActive(false);

        // Ligado ao "Continuar"
        public void Continue() => SceneManager.LoadScene(hubSceneName);

        // Ligado ao "Sair do jogo"
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
