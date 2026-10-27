using UnityEngine;

namespace LastWick.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 5f;

        private Rigidbody2D body;
        private PlayerInput input;

        // A esquiva trava o movimento enquanto controla o corpo.
        public bool Locked { get; set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            if (Locked)
                return;

            body.linearVelocity = Vector2.ClampMagnitude(input.Move, 1f) * speed;
        }
    }
}
