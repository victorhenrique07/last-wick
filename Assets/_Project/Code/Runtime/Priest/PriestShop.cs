using System;
using Assets._Project.Code.Runtime.Player;
using Assets._Project.Code.Runtime.Save;

namespace Assets._Project.Code.Runtime.Priest
{
    // A compra de pontos no padre, separada da tela. O custo depende só do nível do personagem,
    // então o próximo ponto custa o mesmo em qualquer atributo (PRST-06, PRST-07).
    public static class PriestShop
    {
        public static int LevelOf(AttributeData attribute) => SaveData.GetLevel(SaveSystem.Data.PriestLevels, attribute.Id);

        public static int CharacterLevel() => PermanentStats.CharacterLevel(SaveSystem.Data);

        // Cada ponto custa PriestCostStep vezes o anterior: 5, 9, 16, 29...
        public static int NextCost()
        {
            var db = SaveSystem.Database;
            double cost = db.PriestBaseCost * Math.Pow(db.PriestCostStep, CharacterLevel());

            // Em níveis muito altos a conta passaria do int e viraria negativa; o custo fica no máximo
            return cost >= int.MaxValue ? int.MaxValue : (int)Math.Round(cost, MidpointRounding.ToEven);
        }

        public static bool CanBuy() => SaveSystem.Data.Resources.Gold >= NextCost();

        // Cobra o ouro, sobe o nível e grava; sem ouro, não muda nada (PRST-04, PRST-05, PRST-25)
        public static bool TryBuy(AttributeData attribute)
        {
            if (attribute == null || string.IsNullOrEmpty(attribute.Id))
                return false;

            if (!SaveSystem.TrySpend(new ResourceBag { Gold = NextCost() }))
                return false;

            var levels = SaveSystem.Data.PriestLevels;
            SaveData.SetLevel(levels, attribute.Id, SaveData.GetLevel(levels, attribute.Id) + 1);
            SaveSystem.NotifyChanged();
            SaveSystem.Save();
            return true;
        }
    }
}
