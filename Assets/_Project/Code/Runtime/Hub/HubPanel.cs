using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Project.Code.Runtime.Hub
{
    // Painel do hub (expedição, padre, fogueira, mesa de tarot, ferreiro): um aberto por vez, Esc fecha,
    // e o fundo em tela cheia dentro do root bloqueia os cliques e o contorno no mundo (AD-011).
    public class HubPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root; // fundo escuro + painel

        public static HubPanel Current { get; private set; }

        public event Action Opened;
        public event Action Closed;

        void Awake() => root.SetActive(false);

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        void Update()
        {
            if (Current == this && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Close();
        }

        public void Open()
        {
            if (Current == this) return;
            if (Current != null) Current.Close();

            Current = this;
            root.SetActive(true);
            Opened?.Invoke();
        }

        public void Close()
        {
            if (Current != this) return;

            Current = null;
            root.SetActive(false);
            Closed?.Invoke();
        }

        // Current é estático e o projeto não recarrega o domínio ao dar Play
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Current = null;
    }
}
