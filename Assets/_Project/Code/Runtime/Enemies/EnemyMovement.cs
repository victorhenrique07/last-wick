using UnityEngine;

namespace LastWick.Enemies
{
    // Persegue o player o tempo todo, em linha reta.
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 2f;

        // Compartilhado por todos os inimigos, para não procurar o player em cada um.
        private static Transform player;

        private Rigidbody2D body;

        private void Awake() => body = GetComponent<Rigidbody2D>();

        private void FixedUpdate()
        {
            if (player == null)
            {
                GameObject found = GameObject.FindWithTag("Player");
                if (found == null)
                {
                    body.linearVelocity = Vector2.zero;
                    return;
                }
                player = found.transform;
            }

            Vector2 direction = ((Vector2)player.position - body.position).normalized;
            body.linearVelocity = direction * speed;
        }
    }
}
