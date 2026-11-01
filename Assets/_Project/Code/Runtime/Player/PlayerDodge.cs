using System.Collections;
using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    // Esquiva no estilo soulslike: um deslocamento curto com invencibilidade e cooldown.
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerRunStats))]
    public class PlayerDodge : MonoBehaviour
    {
        [Tooltip("Quanto tempo o deslocamento leva, em segundos.")]
        [SerializeField, Min(0.01f)] private float dashDuration = 0.2f;

        private Rigidbody2D body;
        private PlayerInput input;
        private PlayerMovement movement;
        private PlayerAim aim;
        private Health health;
        private PlayerRunStats stats;
        private float readyTime;

        public bool IsDodging { get; private set; }
        public float CooldownRemaining => Mathf.Max(0f, readyTime - Time.time);

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInput>();
            movement = GetComponent<PlayerMovement>();
            aim = GetComponent<PlayerAim>();
            health = GetComponent<Health>();
            stats = GetComponent<PlayerRunStats>();
        }

        private void Update()
        {
            if (input.DodgePressed && !IsDodging && Time.time >= readyTime)
                StartCoroutine(Dodge());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            IsDodging = false;
            movement.Locked = false;
            if (health != null)
                health.IsInvulnerable = false;
        }

        private IEnumerator Dodge()
        {
            // Esquiva para onde o jogador está andando; parado, esquiva para onde está mirando.
            Vector2 direction = input.Move.sqrMagnitude > 0.01f
                ? input.Move.normalized
                : aim != null ? aim.Direction : Vector2.right;

            readyTime = Time.time + stats.DodgeCooldown;
            IsDodging = true;
            movement.Locked = true;
            if (health != null)
                health.IsInvulnerable = true;

            body.linearVelocity = direction * (stats.DodgeDistance / dashDuration);
            yield return new WaitForSeconds(dashDuration);

            body.linearVelocity = Vector2.zero;
            movement.Locked = false;
            IsDodging = false;

            float remaining = stats.DodgeInvulnerability - dashDuration;
            if (remaining > 0f)
                yield return new WaitForSeconds(remaining);

            if (health != null)
                health.IsInvulnerable = false;
        }
    }
}
