using UnityEngine;

namespace LastWick.Combat
{
    // Arma que dispara projéteis numa direção, respeitando a cadência de tiro.
    // Os números vêm do WeaponData equipado; quem decide quando e para onde atirar é o dono (PlayerAttack, IA de boss etc.).
    public class Weapon : MonoBehaviour
    {
        [Tooltip("De onde o projétil sai. Vazio = posição da própria arma.")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private Team team = Team.Player;

        private WeaponData data;
        private int level;
        private WeaponStats stats;
        private float nextFireTime;
        private bool missingWeaponLogged;

        public WeaponData Data => data;
        public int Level => level;
        public WeaponStats Stats => stats;

        // Dano da arma no nível, antes do padre e das cartas
        public float BaseDamage => stats.Damage;

        // Equipa a arma no nível salvo; os status somam os níveis 1 até o nível (SMITH-08, SMITH-09)
        public void Equip(WeaponData weaponData, int weaponLevel)
        {
            data = weaponData;
            level = weaponData != null ? Mathf.Clamp(weaponLevel, 0, weaponData.MaxLevel) : 0;
            stats = weaponData != null ? WeaponStats.Build(weaponData, level) : default;
        }

        // Os multiplicadores recebidos são os das cartas; os da arma multiplicam por cima e a perfuração soma (SMITH-10)
        public bool TryFire(Vector2 direction, float finalDamage, float fireRateMultiplier = 1f,
            float projectileSpeedMultiplier = 1f, float projectileLifetimeMultiplier = 1f, int bonusPierce = 0)
        {
            if (data == null || data.ProjectilePrefab == null)
            {
                if (!missingWeaponLogged)
                {
                    Debug.LogError($"Weapon '{name}': nenhuma arma equipada (ou arma sem projétil); não atira.", this);
                    missingWeaponLogged = true;
                }
                return false;
            }

            if (direction == Vector2.zero || Time.time < nextFireTime)
                return false;

            nextFireTime = Time.time + 1f / (stats.FireRate * fireRateMultiplier);

            Transform origin = muzzle != null ? muzzle : transform;
            Projectile projectile = Instantiate(data.ProjectilePrefab, origin.position, Quaternion.identity);
            projectile.Launch(direction.normalized, team, finalDamage,
                projectileSpeedMultiplier * stats.ProjectileSpeedMultiplier,
                projectileLifetimeMultiplier * stats.LifetimeMultiplier,
                bonusPierce + stats.BonusPierce);
            return true;
        }
    }
}
