using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets._Project.Code.Runtime.Save
{
    [Serializable]
    public class SaveData
    {
        public int Version = 1;
        public ResourceBag Resources;
        public List<IdLevel> PriestLevels = new();
        public int LightLevel;
        public List<string> UnlockedCards = new();
        public List<string> Deck = new();
        public List<IdLevel> WeaponLevels = new();
        public string EquippedWeapon;

        public static SaveData CreateNew(GameDatabase db)
        {
            var saveData = new SaveData
            {
                Resources = new ResourceBag(),
                PriestLevels = new List<IdLevel>(),
                LightLevel = 0,
                UnlockedCards = db.StartingDeck.Cards.Select(c => c.Id).ToList(),
                Deck = db.StartingDeck.Cards.Select(c => c.Id).ToList(),
                WeaponLevels = new List<IdLevel>(),
                EquippedWeapon = StartingWeaponId(db)
            };

            return saveData;
        }

        // Save novo já começa com a arma inicial equipada (SAVE-22)
        private static string StartingWeaponId(GameDatabase db)
        {
            if (db.Weapons == null || db.Weapons.StartingWeapon == null) return null;
            return db.Weapons.StartingWeapon.Id;
        }

        public static int GetLevel(List<IdLevel> list, string id)
        {
            var item = list.FirstOrDefault(x => x.Id == id);
            return item.Equals(default(IdLevel)) ? 0 : item.Level;
        }

        public static void SetLevel(List<IdLevel> list, string id, int level)
        {
            var index = list.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                list[index] = new IdLevel { Id = id, Level = level };
            }
            else
            {
                list.Add(new IdLevel { Id = id, Level = level });
            }
        }
    }

    [Serializable]
    public struct IdLevel
    {
        // O Id armazena o que foi desbloqueado/melhorado.
        //  EX: Upou vida no padre: { Id: "vida", Level: 2 }
        public String Id;
        public int Level;
    }
}
