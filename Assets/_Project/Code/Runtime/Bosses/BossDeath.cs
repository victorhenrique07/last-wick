using LastWick.Combat;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Bosses
{
    // Avisa a run que um boss morreu, para o Spawner passar ao próximo estágio.
    [RequireComponent(typeof(Health))]
    public class BossDeath : MonoBehaviour
    {
        [Tooltip("Índice do boss: 0 = Boss1, 1 = Boss2, 2 = FinalBoss.")]
        [SerializeField, Min(0)] private int bossIndex;

        private Health health;

        void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            health.Died += OnDied;
        }

        private void OnDisable()
        {
            health.Died -= OnDied;
        }

        private void OnDied()
        {
            GameEvents.RaiseBossDefeated(bossIndex);
        }
    }
}
