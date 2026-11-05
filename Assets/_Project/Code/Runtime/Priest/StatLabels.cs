using System;
using Assets._Project.Code.Runtime.Player;

namespace Assets._Project.Code.Runtime.Priest
{
    // Nome e formato de cada status no menu do padre. Um status novo no enum ganha uma linha aqui.
    public static class StatLabels
    {
        public static string Name(PlayerStatType stat) => stat switch
        {
            PlayerStatType.MaxHealth => "Vida máxima",
            PlayerStatType.Damage => "Dano base",
            PlayerStatType.MoveSpeed => "Velocidade",
            PlayerStatType.DodgeDistance => "Distância da esquiva",
            PlayerStatType.DodgeCooldown => "Recarga da esquiva",
            PlayerStatType.FireRate => "Cadência",
            PlayerStatType.XpPickupRadius => "Coleta de XP",
            PlayerStatType.Defense => "Defesa",
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };

        // Defesa e cadência em porcentagem (PRST-19); a recarga em segundos
        public static string Format(PlayerStatType stat, float value) => stat switch
        {
            PlayerStatType.Defense => $"{value * 100f:0.#}%",
            PlayerStatType.FireRate => $"{value * 100f:0}%",
            PlayerStatType.DodgeCooldown => $"{value:0.##}s",
            PlayerStatType.MaxHealth => $"{value:0}",
            _ => $"{value:0.##}"
        };
    }
}
