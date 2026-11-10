using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerRunStats))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Knockback ao tomar dano")]
        [Tooltip("Velocidade do empurrão no instante do hit.")]
        [SerializeField, Min(0f)] private float knockbackForce = 4f;
        [Tooltip("Quanto da velocidade do empurrão some por segundo. Mais alto = empurrão mais curto e seco.")]
        [SerializeField, Min(0f)] private float knockbackDecay = 40f;
        [Tooltip("Segundos logo após o hit em que o input não move o player, para ele não anular o empurrão.")]
        [SerializeField, Min(0f)] private float stunDuration = 0.05f;

        private Rigidbody2D body;
        private PlayerInput input;
        private PlayerRunStats stats;
        private Health health;

        private Vector2 knockbackVelocity;
        private float stunTimer;

        // A esquiva trava o movimento enquanto controla o corpo.
        public bool Locked { get; set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInput>();
            stats = GetComponent<PlayerRunStats>();
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null) health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            if (health != null) health.Damaged -= OnDamaged;
            knockbackVelocity = Vector2.zero;
            stunTimer = 0f;
        }

        // Este script é o único dono da velocidade fora da esquiva. Por isso o empurrão
        // é somado aqui, em vez de um AddForce que seria apagado no frame seguinte.
        private void OnDamaged(DamageInfo info)
        {
            if (info.Direction.sqrMagnitude < 0.0001f) return;

            knockbackVelocity = info.Direction.normalized * knockbackForce;
            stunTimer = stunDuration;
        }

        private void FixedUpdate()
        {
            // Esquivar durante o empurrão cancela ele: a esquiva é a saída do jogador.
            if (Locked)
            {
                knockbackVelocity = Vector2.zero;
                stunTimer = 0f;
                return;
            }

            Vector2 move = Vector2.ClampMagnitude(input.Move, 1f) * stats.MoveSpeed;
            if (stunTimer > 0f)
            {
                stunTimer -= Time.fixedDeltaTime;
                move = Vector2.zero;
            }

            body.linearVelocity = move + knockbackVelocity;
            knockbackVelocity = Vector2.MoveTowards(knockbackVelocity, Vector2.zero, knockbackDecay * Time.fixedDeltaTime);
        }
    }
}
