using System;
using UnityEngine;

namespace LastWick.Combat
{
    // Vida de qualquer entidade: player, inimigos e bosses.
    public class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [Tooltip("Destrói o GameObject ao morrer. Desligue quando outro sistema cuidar da morte (pool, tela de resultado).")]
        [SerializeField] private bool destroyOnDeath;

        public float Max => maxHealth;
        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;

        // Ligado pela esquiva durante os frames de invencibilidade.
        public bool IsInvulnerable { get; set; }

        public event Action<DamageInfo> Damaged;
        public event Action<float> Healed;
        public event Action Died;

        private void OnEnable()
        {
            Current = maxHealth;
            IsInvulnerable = false;
        }

        public void TakeDamage(DamageInfo info)
        {
            if (IsDead || IsInvulnerable || info.Amount <= 0f)
                return;

            Current = Mathf.Max(0f, Current - info.Amount);
            Damaged?.Invoke(info);

            if (IsDead)
            {
                Died?.Invoke();
                if (destroyOnDeath)
                    Destroy(gameObject);
            }
        }

        // Morte imediata, ignorando a invencibilidade (ex.: a escuridão aos 10:00).
        public void Kill()
        {
            if (IsDead)
                return;

            Current = 0f;
            Died?.Invoke();
            if (destroyOnDeath)
                Destroy(gameObject);
        }

        // Muda a vida máxima; o que for ganho entra também na vida atual.
        public void SetMax(float value)
        {
            float gained = value - maxHealth;

            maxHealth = value;
            Current = Mathf.Min(Current, maxHealth);
            Heal(gained);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
                return;

            float before = Current;
            Current = Mathf.Min(maxHealth, Current + amount);
            Healed?.Invoke(Current - before);
        }
    }
}
