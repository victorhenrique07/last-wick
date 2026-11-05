using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Code.Runtime.Priest;
using Assets._Project.Code.Runtime.Save;
using LastWick.Player;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Player
{
    // Status base da run, antes das cartas: PlayerStats + pontos do padre (AD-012).
    public struct StatBlock
    {
        public float MaxHealth;
        public float MoveSpeed;
        public float DodgeDistance;
        public float DodgeInvulnerability;
        public float DodgeCooldown;
        public float Damage;
        public float LightRadius;
        public float XpPickupRadius;
        public float FireRate;        // multiplicador da cadência da arma; começa em 1
        public float DamageReduction; // fração do dano recebido que é tirada (0 a 1)
    }

    // Monta o StatBlock. A run usa os níveis do save; o menu do padre usa os níveis do save com +1
    // num atributo para mostrar o "próximo valor" com a mesma conta.
    public static class PermanentStats
    {
        private const float MinDodgeCooldown = 0.2f; // PRST-26

        public static StatBlock Build(PlayerStats baseStats, IEnumerable<AttributeData> attributes, Func<AttributeData, int> levelOf)
        {
            var block = new StatBlock
            {
                MaxHealth = baseStats.MaxHealth,
                MoveSpeed = baseStats.MoveSpeed,
                DodgeDistance = baseStats.DodgeDistance,
                DodgeInvulnerability = baseStats.DodgeInvulnerability,
                DodgeCooldown = baseStats.DodgeCooldown,
                Damage = baseStats.Damage,
                LightRadius = baseStats.LightRadius,
                XpPickupRadius = baseStats.XpPickupRadius,
                FireRate = 1f,
                DamageReduction = baseStats.DamageReduction
            };

            foreach (var attribute in attributes)
            {
                int level = levelOf(attribute);
                if (level <= 0) continue;

                foreach (var effect in attribute.Effects)
                    Apply(ref block, effect.Stat, effect.GainPerLevel, level);
            }

            block.DodgeCooldown = Mathf.Max(MinDodgeCooldown, block.DodgeCooldown);
            return block;
        }

        public static StatBlock Build(PlayerStats baseStats, SaveData save, GameDatabase database) => 
            Build(baseStats, database.Attributes, attr => SaveData.GetLevel(save.PriestLevels, attr.Id));

        public static float Get(StatBlock block, PlayerStatType stat) => stat switch
        {
            PlayerStatType.MaxHealth => block.MaxHealth,
            PlayerStatType.Damage => block.Damage,
            PlayerStatType.MoveSpeed => block.MoveSpeed,
            PlayerStatType.DodgeDistance => block.DodgeDistance,
            PlayerStatType.DodgeCooldown => block.DodgeCooldown,
            PlayerStatType.FireRate => block.FireRate,
            PlayerStatType.XpPickupRadius => block.XpPickupRadius,
            PlayerStatType.Defense => block.DamageReduction,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };

        // Soma de todos os níveis salvos, inclusive de atributos que não existem mais (PRST-06, PRST-28)
        public static int CharacterLevel(SaveData save) => save.PriestLevels.Sum(entry => entry.Level);

        private static void Apply(ref StatBlock block, PlayerStatType stat, float gain, int level)
        {
            float total = gain * level;

            switch (stat)
            {
                case PlayerStatType.MaxHealth: block.MaxHealth += total; break;
                case PlayerStatType.Damage: block.Damage += total; break;
                case PlayerStatType.MoveSpeed: block.MoveSpeed += total; break;
                case PlayerStatType.DodgeDistance: block.DodgeDistance += total; break;
                case PlayerStatType.DodgeCooldown: block.DodgeCooldown += total; break; // ganho negativo
                case PlayerStatType.FireRate: block.FireRate += total; break;
                case PlayerStatType.XpPickupRadius: block.XpPickupRadius += total; break;

                // A defesa multiplica, como a absorção dos soulslikes: cada ponto tira uma fração do dano que sobrou
                case PlayerStatType.Defense:
                    block.DamageReduction = 1f - (1f - block.DamageReduction) * Mathf.Pow(1f - gain, level);
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }
    }
}
