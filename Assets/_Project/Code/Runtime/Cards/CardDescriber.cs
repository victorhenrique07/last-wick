using System.Collections.Generic;

namespace Assets._Project.Code.Runtime.Cards
{
    // As linhas de efeito de uma carta, uma por bônus diferente de zero (TAROT-08).
    // Os nomes seguem os do padre e da tela de resultado.
    public static class CardDescriber
    {
        public static List<string> Lines(CardData c)
        {
            var lines = new List<string>();
            if (c == null) return lines;

            Percent(lines, c.MaxHealthBonus, "vida máxima");
            Percent(lines, c.MoveSpeedBonus, "velocidade");
            Percent(lines, c.DodgeDistanceBonus, "distância da esquiva");
            Percent(lines, c.DodgeInvulnerabilityBonus, "invencibilidade da esquiva");
            // Redução escrita com sinal de menos (TAROT-22)
            if (c.DodgeCooldownReduction != 0f) lines.Add($"-{c.DodgeCooldownReduction * 100f:0.#}% recarga da esquiva");
            // Dano fixo e perfuração são quantidades, não porcentagens
            if (c.BaseDamageBonus != 0f) lines.Add($"+{c.BaseDamageBonus:0.#} dano base");
            Percent(lines, c.DamageBonus, "dano");
            Percent(lines, c.FireRateBonus, "cadência");
            Percent(lines, c.ProjectileSpeedBonus, "velocidade do projétil");
            Percent(lines, c.ProjectileLifetimeBonus, "alcance do projétil");
            if (c.PierceBonus != 0) lines.Add($"+{c.PierceBonus} perfuração");
            Percent(lines, c.LightRadiusBonus, "raio da luz");
            Percent(lines, c.XpPickupRadiusBonus, "coleta de XP");

            return lines;
        }

        // 0,2 vira "+20% nome"
        private static void Percent(List<string> lines, float value, string name)
        {
            if (value != 0f) lines.Add($"+{value * 100f:0.#}% {name}");
        }
    }
}
