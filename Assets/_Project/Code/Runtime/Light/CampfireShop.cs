using Assets._Project.Code.Runtime.Save;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Light
{
    // A compra de luz na fogueira, separada da tela: um nível por vez, pago com óleo.
    public static class CampfireShop
    {
        private static LightConfig Config => SaveSystem.Database.Light;

        // Preso em 0..máximo, mesmo que o save tenha mudado depois de carregar
        public static int Level => Mathf.Clamp(SaveSystem.Data.LightLevel, 0, Config.MaxLevel);

        public static bool IsMaxed => Level >= Config.MaxLevel; // LIGHT-16

        // Custo do próximo nível, em óleo (LIGHT-04); 0 quando a luz já está no máximo
        public static int NextCost => IsMaxed ? 0 : LightRules.CostOf(Config, Level + 1);

        public static bool CanBuy => !IsMaxed && SaveSystem.Data.Resources.Oil >= NextCost;

        // Cobra o óleo, sobe o nível e grava; sem óleo ou no máximo, não muda nada (LIGHT-05, LIGHT-06)
        public static bool TryBuy()
        {
            if (IsMaxed) return false;
            if (!SaveSystem.TrySpend(new ResourceBag { Oil = NextCost })) return false;

            SaveSystem.Data.LightLevel = Level + 1;
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
            return true;
        }
    }
}
