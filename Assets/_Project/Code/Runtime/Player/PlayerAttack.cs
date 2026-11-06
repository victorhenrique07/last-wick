using Assets._Project.Code.Runtime.Save;
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

        // Equipa a arma do save no nível comprado no ferreiro (SMITH-08, SMITH-14)
        private void Start()
        {
            if (weapon == null) return;

            string id = SaveSystem.Data.EquippedWeapon;
            WeaponData data = SaveSystem.Database.FindWeapon(id);
            if (data == null)
            {
                Debug.LogError($"PlayerAttack: a arma equipada '{id}' não está no catálogo de armas.", this);
                return;
            }

            weapon.Equip(data, SaveData.GetLevel(SaveSystem.Data.WeaponLevels, data.Id));
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
