using UnityEngine;

namespace LastWick.Combat
{
    // Arma que dispara projéteis numa direção, respeitando a cadência de tiro.
    // Quem decide quando e para onde atirar é o dono (PlayerAttack, IA de boss etc.).
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [Tooltip("De onde o projétil sai. Vazio = posição da própria arma.")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private Team team = Team.Player;
        [SerializeField, Min(0f)] private float damage = 10f;
        [Tooltip("Disparos por segundo.")]
        [SerializeField, Min(0.01f)] private float fireRate = 4f;

        private float nextFireTime;

        public bool TryFire(Vector2 direction)
        {
            if (projectilePrefab == null || direction == Vector2.zero || Time.time < nextFireTime)
                return false;

            nextFireTime = Time.time + 1f / fireRate;

            Transform origin = muzzle != null ? muzzle : transform;
            Projectile projectile = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
            projectile.Launch(direction.normalized, team, damage);
            return true;
        }
    }
}
