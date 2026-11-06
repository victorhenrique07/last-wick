using LastWick.Combat;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Cards
{
    // Base de toda carta com mecânica própria. O CardEffects cria uma instância por carta, como filha do player,
    // e avisa a cada cópia nova; a mecânica decide se a cópia vira mais uma instância do que ela faz ou mais força.
    //
    // Regras para quem escreve uma mecânica:
    // - entre nos eventos no OnEnable e saia no OnDisable: na morte do player o efeito é desligado (enabled = false);
    // - use só tempo escalado (Time.deltaTime, WaitForSeconds, Invoke), para parar com o level up e o resultado abertos;
    // - pegue os componentes do player pelo Player (PlayerRunStats, Health, PlayerDodge...), não procurando na cena.
    public abstract class CardEffect : MonoBehaviour
    {
        private Health playerHealth;

        protected GameObject Player { get; private set; } // CEFX-08
        protected CardData Card { get; private set; }
        protected int Copies { get; private set; }

        public void Attach(GameObject player, CardData card)
        {
            Player = player;
            Card = card;

            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Died += HandlePlayerDied;
        }

        // CEFX-02, CEFX-03: a primeira cópia chega como 1, as seguintes com o total
        public void SetCopies(int copies)
        {
            Copies = copies;
            OnCopiesChanged(copies);
        }

        protected virtual void OnCopiesChanged(int copies) { }

        protected virtual void OnPlayerDied() { }

        // CEFX-12: nenhum efeito age depois da morte do player
        private void HandlePlayerDied()
        {
            enabled = false;
            OnPlayerDied();
        }

        // O Health do player pode viver mais que o efeito
        protected virtual void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.Died -= HandlePlayerDied;
        }
    }
}
