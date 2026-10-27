using UnityEngine;

namespace LastWick.Combat
{
    // Área que recebe dano. Fica no mesmo GameObject do collider e repassa o dano para o Health.
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] private Team team;
        [SerializeField] private Health health;

        public Team Team => team;

        private void Reset() => health = GetComponentInParent<Health>();

        private void Awake()
        {
            if (health == null)
                health = GetComponentInParent<Health>();
        }

        public void Receive(DamageInfo info)
        {
            if (health != null)
                health.TakeDamage(info);
        }
    }
}
