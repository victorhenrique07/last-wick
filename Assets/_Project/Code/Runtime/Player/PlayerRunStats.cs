using System;
using Assets._Project.Code.Runtime.Cards;
using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    // Status do player na run atual: os valores base mais os bônus das cartas.
    // Só valem durante a partida e nunca voltam para o PlayerStats.
    [RequireComponent(typeof(CardInventory), typeof(Health), typeof(PlayerLight))]
    public class PlayerRunStats : MonoBehaviour
    {
        [SerializeField] private PlayerStats baseStats;

        private CardInventory inventory;
        private Health health;
        private PlayerLight playerLight;
        private float flatDamage;
        private float damageMultiplier;

        public float MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }
        public float DodgeDistance { get; private set; }
        public float DodgeInvulnerability { get; private set; }
        public float DodgeCooldown { get; private set; }
        public float LightRadius { get; private set; }
        public float XpPickupRadius { get; private set; }

        // A base destes fica na arma e no projétil.
        public float FireRateMultiplier { get; private set; }
        public float ProjectileSpeedMultiplier { get; private set; }
        public float ProjectileLifetimeMultiplier { get; private set; }
        public int BonusPierce { get; private set; }

        private void Awake()
        {
            inventory = GetComponent<CardInventory>();
            health = GetComponent<Health>();
            playerLight = GetComponent<PlayerLight>();
            Refresh();
        }

        private void OnEnable() => inventory.Changed += Refresh;
        private void OnDisable() => inventory.Changed -= Refresh;

        public float CalculateDamage(float weaponDamage)
        {
            return (baseStats.Damage + weaponDamage + flatDamage) * damageMultiplier;
        }

        private void Refresh()
        {
            MaxHealth = baseStats.MaxHealth * Multiplier(card => card.MaxHealthBonus);
            MoveSpeed = baseStats.MoveSpeed * Multiplier(card => card.MoveSpeedBonus);

            DodgeDistance = baseStats.DodgeDistance * Multiplier(card => card.DodgeDistanceBonus);
            DodgeInvulnerability = baseStats.DodgeInvulnerability * Multiplier(card => card.DodgeInvulnerabilityBonus);
            DodgeCooldown = baseStats.DodgeCooldown * inventory.ReductionMultiplier(card => card.DodgeCooldownReduction);

            LightRadius = baseStats.LightRadius * Multiplier(card => card.LightRadiusBonus);
            XpPickupRadius = baseStats.XpPickupRadius * Multiplier(card => card.XpPickupRadiusBonus);

            flatDamage = inventory.Total(card => card.BaseDamageBonus);
            damageMultiplier = Multiplier(card => card.DamageBonus);

            FireRateMultiplier = Multiplier(card => card.FireRateBonus);
            ProjectileSpeedMultiplier = Multiplier(card => card.ProjectileSpeedBonus);
            ProjectileLifetimeMultiplier = Multiplier(card => card.ProjectileLifetimeBonus);
            BonusPierce = Mathf.RoundToInt(inventory.Total(card => card.PierceBonus));

            health.SetMax(MaxHealth);
            playerLight.Radius = LightRadius;
        }

        private float Multiplier(Func<CardData, float> bonus) => 1f + inventory.Total(bonus);
    }
}
