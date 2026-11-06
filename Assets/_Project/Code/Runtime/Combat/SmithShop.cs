using Assets._Project.Code.Runtime.Save;
using UnityEngine;

namespace LastWick.Combat
{
    // A melhoria da arma no ferreiro, separada da tela: um nível por vez, pago com osso, ferro e cera.
    public static class SmithShop
    {
        // A arma equipada no save; o save já troca uma arma desconhecida pela inicial (SAVE-22)
        public static WeaponData Equipped => SaveSystem.Database.FindWeapon(SaveSystem.Data.EquippedWeapon);

        // Preso em 0..máximo, mesmo que o save tenha um nível acima (SMITH-17)
        public static int Level
        {
            get
            {
                WeaponData weapon = Equipped;
                if (weapon == null) return 0;
                return Mathf.Clamp(SaveData.GetLevel(SaveSystem.Data.WeaponLevels, weapon.Id), 0, weapon.MaxLevel);
            }
        }

        public static bool IsMaxed => Equipped == null || Level >= Equipped.MaxLevel; // SMITH-15

        // Custo do próximo nível (SMITH-04); vazio quando a arma já está no máximo
        public static ResourceBag NextCost => IsMaxed ? default : Equipped.Levels[Level].Cost;

        public static bool CanUpgrade => !IsMaxed && SaveSystem.Data.Resources.Covers(NextCost);

        // Cobra os materiais, sobe um nível e grava; sem materiais ou no máximo, não muda nada (SMITH-05, SMITH-06, SMITH-18)
        public static bool TryUpgrade()
        {
            if (IsMaxed) return false;

            WeaponData weapon = Equipped;
            int level = Level;
            if (!SaveSystem.TrySpend(weapon.Levels[level].Cost)) return false;

            SaveData.SetLevel(SaveSystem.Data.WeaponLevels, weapon.Id, level + 1);
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
            return true;
        }
    }
}
