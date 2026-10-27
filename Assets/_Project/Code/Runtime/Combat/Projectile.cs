using UnityEngine;

namespace LastWick.Combat
{
    // Projétil que anda em linha reta e causa dano pelo Hitbox.
    [RequireComponent(typeof(Rigidbody2D), typeof(Hitbox))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 12f;
        [SerializeField, Min(0.01f)] private float lifetime = 2f;
        [Tooltip("Quantos alvos atravessa antes de sumir. 0 = some no primeiro acerto.")]
        [SerializeField, Min(0)] private int pierce;

        private Rigidbody2D body;
        private Hitbox hitbox;
        private int hitsLeft;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            hitbox = GetComponent<Hitbox>();
            hitbox.Hit += OnHit;
        }

        public void Launch(Vector2 direction, Team team, float damage)
        {
            hitbox.Team = team;
            hitbox.Damage = damage;
            hitsLeft = pierce;

            transform.right = direction;
            body.linearVelocity = direction.normalized * speed;
            Destroy(gameObject, lifetime);
        }

        private void OnHit(Hurtbox target)
        {
            if (hitsLeft-- <= 0)
                Destroy(gameObject);
        }
    }
}
