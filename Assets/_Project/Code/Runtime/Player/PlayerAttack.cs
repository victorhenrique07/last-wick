using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    // Ataque manual: enquanto o botão estiver segurado, dispara a arma na direção da mira.
    [RequireComponent(typeof(PlayerInput), typeof(PlayerAim), typeof(PlayerRunStats))]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        private PlayerInput input;
        private PlayerAim aim;
        private PlayerRunStats stats;

        private void Reset() => weapon = GetComponentInChildren<Weapon>();

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            aim = GetComponent<PlayerAim>();
            stats = GetComponent<PlayerRunStats>();
        }

        private void Update()
        {
            if (weapon != null && input.AttackHeld)
            {
                weapon.TryFire(aim.Direction, stats.CalculateDamage(weapon.BaseDamage), stats.FireRateMultiplier,
                    stats.ProjectileSpeedMultiplier, stats.ProjectileLifetimeMultiplier, stats.BonusPierce);
            }
        }
    }
}
