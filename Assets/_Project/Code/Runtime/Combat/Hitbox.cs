using System;
using System.Collections.Generic;
using UnityEngine;

namespace LastWick.Combat
{
    // Área que causa dano. Precisa de um collider marcado como trigger.
    // Usada pelos projéteis e pelo dano de contato dos inimigos.
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private Team team;
        [SerializeField, Min(0f)] private float damage = 10f;
        [Tooltip("Intervalo entre acertos no mesmo alvo enquanto ele continuar encostado. 0 = acerta cada alvo uma vez só.")]
        [SerializeField, Min(0f)] private float repeatInterval;

        private readonly Dictionary<Hurtbox, float> nextHitTime = new();

        public event Action<Hurtbox> Hit;

        public Team Team
        {
            get => team;
            set => team = value;
        }

        public float Damage
        {
            get => damage;
            set => damage = value;
        }

        private void OnEnable() => nextHitTime.Clear();

        private void OnTriggerEnter2D(Collider2D other) => TryHit(other);

        private void OnTriggerStay2D(Collider2D other)
        {
            if (repeatInterval > 0f)
                TryHit(other);
        }

        private void TryHit(Collider2D other)
        {
            if (!other.TryGetComponent(out Hurtbox hurtbox) || hurtbox.Team == team)
                return;
            if (nextHitTime.TryGetValue(hurtbox, out float next) && Time.time < next)
                return;

            nextHitTime[hurtbox] = repeatInterval > 0f ? Time.time + repeatInterval : float.PositiveInfinity;

            Vector2 direction = ((Vector2)(hurtbox.transform.position - transform.position)).normalized;
            hurtbox.Receive(new DamageInfo(damage, direction, gameObject));
            Hit?.Invoke(hurtbox);
        }
    }
}
