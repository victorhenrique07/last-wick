using UnityEngine;

namespace Assets._Project.Code.Runtime.Light
{
    // Contas puras da fogueira: o raio de cada nível e o custo de cada nível em óleo.
    public static class LightRules
    {
        // Raio da luz do player num nível da fogueira (LIGHT-08); um nível acima do máximo fica no máximo
        public static float Radius(LightConfig config, int level)
        {
            int clamped = Mathf.Clamp(level, 0, config.MaxLevel);
            return config.BaseRadius + config.RadiusPerLevel * clamped;
        }

        // Custo para comprar o nível `level`, em óleo (LIGHT-04); o nível 0 é o inicial e não se compra
        public static int CostOf(LightConfig config, int level)
        {
            if (level <= 0) return 0;
            return config.BaseCost + config.CostStep * (level - 1);
        }
    }
}
