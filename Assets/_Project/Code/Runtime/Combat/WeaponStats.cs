using UnityEngine;

namespace LastWick.Combat
{
    // Os números da arma num nível, antes das cartas: a base do asset mais os níveis 1 até o nível (SMITH-09).
    public struct WeaponStats
    {
        public float Damage;
        public float FireRate;
        public float ProjectileSpeedMultiplier;
        public float LifetimeMultiplier;
        public int BonusPierce;

        public static WeaponStats Build(WeaponData data, int level)
        {
            if (data == null)
            {
                Debug.LogError("WeaponStats: nenhuma arma informada; os status ficam zerados.");
                return default;
            }

            // SMITH-17: nível salvo acima do máximo usa o último
            level = Mathf.Clamp(level, 0, data.MaxLevel);

            float damageBonus = 0f, fireRateBonus = 0f, speedBonus = 0f, lifetimeBonus = 0f;
            int pierceBonus = 0;

            for (int i = 0; i < level; i++)
            {
                WeaponLevel step = data.Levels[i];
                if (step == null) continue;

                damageBonus += step.DamageBonus;
                fireRateBonus += step.FireRateBonus;
                speedBonus += step.ProjectileSpeedBonus;
                lifetimeBonus += step.LifetimeBonus;
                pierceBonus += step.PierceBonus;
            }

            // Cadência: frações somadas, não multiplicadas nível a nível (design, Tech Decisions)
            return new WeaponStats
            {
                Damage = data.BaseDamage + damageBonus,
                FireRate = data.BaseFireRate * (1f + fireRateBonus),
                ProjectileSpeedMultiplier = 1f + speedBonus,
                LifetimeMultiplier = 1f + lifetimeBonus,
                BonusPierce = pierceBonus
            };
        }
    }
}
