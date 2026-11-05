using System;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Player;
using Assets._Project.Code.Runtime.Save;
using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    // Status do player na run atual: a base permanente (PlayerStats + padre) mais os bônus das cartas.
    // Só valem durante a partida e nunca voltam para o PlayerStats.
    [RequireComponent(typeof(CardInventory), typeof(Health), typeof(PlayerLight))]
    public class PlayerRunStats : MonoBehaviour
    {
        [SerializeField] private PlayerStats baseStats;

        private CardInventory inventory;
        private Health health;
        private PlayerLight playerLight;
        private StatBlock block; // montado uma vez por run: os pontos do padre não mudam durante a partida
        private float flatDamage;
        private float damageMultiplier;

        public float MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }
        public float DodgeDistance { get; private set; }
        public float DodgeInvulnerability { get; private set; }
        public float DodgeCooldown { get; private set; }
        public float LightRadius { get; private set; }
        public float XpPickupRadius { get; private set; }
        public float DamageReduction { get; private set; }

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
            block = PermanentStats.Build(baseStats, SaveSystem.Data, SaveSystem.Database);
            Refresh();
        }

        private void OnEnable() => inventory.Changed += Refresh;
        private void OnDisable() => inventory.Changed -= Refresh;

        public float CalculateDamage(float weaponDamage)
        {
            return (block.Damage + weaponDamage + flatDamage) * damageMultiplier;
        }

        private void Refresh()
        {
            MaxHealth = block.MaxHealth * Multiplier(card => card.MaxHealthBonus);
            MoveSpeed = block.MoveSpeed * Multiplier(card => card.MoveSpeedBonus);

            DodgeDistance = block.DodgeDistance * Multiplier(card => card.DodgeDistanceBonus);
            DodgeInvulnerability = block.DodgeInvulnerability * Multiplier(card => card.DodgeInvulnerabilityBonus);
            DodgeCooldown = block.DodgeCooldown * inventory.ReductionMultiplier(card => card.DodgeCooldownReduction);
            DodgeCooldown = Mathf.Max(PermanentStats.MinDodgeCooldown, DodgeCooldown); // as cartas reduzem de novo por cima

            LightRadius = block.LightRadius * Multiplier(card => card.LightRadiusBonus);
            XpPickupRadius = block.XpPickupRadius * Multiplier(card => card.XpPickupRadiusBonus);
            DamageReduction = block.DamageReduction;

            flatDamage = inventory.Total(card => card.BaseDamageBonus);
            damageMultiplier = Multiplier(card => card.DamageBonus);

            FireRateMultiplier = block.FireRate * Multiplier(card => card.FireRateBonus);
            ProjectileSpeedMultiplier = Multiplier(card => card.ProjectileSpeedBonus);
            ProjectileLifetimeMultiplier = Multiplier(card => card.ProjectileLifetimeBonus);
            BonusPierce = Mathf.RoundToInt(inventory.Total(card => card.PierceBonus));

            health.SetMax(MaxHealth);
            health.DamageReductionMultiplier = 1f - DamageReduction;
            playerLight.Radius = LightRadius;
        }

        private float Multiplier(Func<CardData, float> bonus) => 1f + inventory.Total(bonus);
    }
}
