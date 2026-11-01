using UnityEngine;

namespace LastWick.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerRunStats))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D body;
        private PlayerInput input;
        private PlayerRunStats stats;

        // A esquiva trava o movimento enquanto controla o corpo.
        public bool Locked { get; set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInput>();
            stats = GetComponent<PlayerRunStats>();
        }

        private void FixedUpdate()
        {
            if (Locked)
                return;

            body.linearVelocity = Vector2.ClampMagnitude(input.Move, 1f) * stats.MoveSpeed;
        }
    }
}
