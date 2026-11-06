using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LastWick.Combat;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Save
{
    public static class SaveSystem
    {
        private static GameDatabase database;
        private static SaveData data;

        public static GameDatabase Database
        {
            get
            {
                if (database == null) database = Resources.Load<GameDatabase>("GameDatabase");
                return database;
            }
        }

        public static SaveData Data
        {
            get
            {
                if (data == null) data = Load();
                return data;
            }
        }

        public static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
        public static string CorruptPath => Path.Combine(Application.persistentDataPath, "save.corrupt.json");

        // O menu principal usa para habilitar o "Continuar"
        public static bool HasSave => File.Exists(FilePath);

        public static event Action Changed;
        public static void NotifyChanged() => Changed?.Invoke();

        // Novo jogo: descarta o progresso atual e grava um save novo no lugar
        public static void StartNew()
        {
            data = SaveData.CreateNew(Database);
            Save();
            NotifyChanged();
        }

        private static SaveData Load()
        {
            if (!File.Exists(FilePath)) 
                return SaveData.CreateNew(Database);

            try
            {
                string text = File.ReadAllText(FilePath);

                SaveData saveData = JsonUtility.FromJson<SaveData>(text);

                if (saveData == null || saveData.Version > 1)
                    return MoveToCorruptAndStartOver("Save data is incompatible with the current version of the game.");

                Sanitize(saveData);

                return saveData;
            }
            catch (Exception e)
            {
                return MoveToCorruptAndStartOver(e.Message);
            }
        }

        private static void Sanitize(SaveData saveData)
        {
            if (saveData.PriestLevels == null)
                saveData.PriestLevels = new List<IdLevel>();
            if (saveData.WeaponLevels == null)
                saveData.WeaponLevels = new List<IdLevel>();
            if (saveData.UnlockedCards == null)
                saveData.UnlockedCards = new List<string>();
            if (saveData.Deck == null)
                saveData.Deck = new List<string>();

            saveData.Resources = saveData.Resources.ClampedToZero();

            // Nível de luz fora do intervalo (save editado ou MaxLevel reduzido) volta para 0..máximo
            int maxLight = Database.Light != null ? Database.Light.MaxLevel : int.MaxValue;
            saveData.LightLevel = Math.Clamp(saveData.LightLevel, 0, maxLight);

            // Id vazio nunca é uma carta, e cada carta aparece uma vez só (TAROT-20)
            saveData.UnlockedCards = saveData.UnlockedCards.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();

            var nonexistentCards = saveData.UnlockedCards.Except(saveData.UnlockedCards.Where(id => Database.FindCard(id) != null)).ToList();
            if (nonexistentCards.Any())
            {
                foreach (var cardId in nonexistentCards)
                {
                    Debug.LogWarning($"Card with ID '{cardId}' does not exist in the database and will be removed from the save data.");
                }

                Debug.LogWarning($"Found {nonexistentCards.Count} nonexistent cards in the save data.");
                saveData.UnlockedCards = saveData.UnlockedCards.Except(nonexistentCards).ToList();
            }

            saveData.Deck = saveData.Deck.Where(card => saveData.UnlockedCards.Contains(card)).Distinct().ToList();

            SanitizeWeapons(saveData);
        }

        // Armas: Id vazio, repetido ou desconhecido sai de WeaponLevels; o repetido fica com a primeira entrada.
        // Arma equipada vazia ou desconhecida vira a arma inicial (SAVE-22).
        private static void SanitizeWeapons(SaveData saveData)
        {
            WeaponCatalog catalog = Database.Weapons;
            if (catalog == null) return;

            var seen = new HashSet<string>();
            var kept = new List<IdLevel>();

            foreach (IdLevel entry in saveData.WeaponLevels)
            {
                if (string.IsNullOrWhiteSpace(entry.Id)) continue;

                if (catalog.Find(entry.Id) == null)
                {
                    Debug.LogWarning($"Weapon with ID '{entry.Id}' does not exist in the database and will be removed from the save data.");
                    continue;
                }

                if (seen.Add(entry.Id))
                    kept.Add(entry);
            }

            saveData.WeaponLevels = kept;

            if (catalog.Find(saveData.EquippedWeapon) == null && catalog.StartingWeapon != null)
                saveData.EquippedWeapon = catalog.StartingWeapon.Id;
        }

        private static SaveData MoveToCorruptAndStartOver(string reason)
        {
            if (File.Exists(CorruptPath))
                File.Delete(CorruptPath);

            File.Move(FilePath, CorruptPath);
            Debug.LogWarning($"Save data is corrupt: {reason}. The save file has been moved to {CorruptPath}.");

            return SaveData.CreateNew(Database);
        }

        public static void Add(ResourceBag amount)
        {
            Data.Resources += amount;
            NotifyChanged();
        }

        public static bool TrySpend(ResourceBag cost)
        {
            if (!Data.Resources.Covers(cost))
                return false;

            Data.Resources -= cost;
            NotifyChanged();

            return true;
        }

        public static void Save()
        {
            string json = JsonUtility.ToJson(Data, true);

            try
            {
                var temp = FilePath + ".temp";
                File.WriteAllText(temp, json);

                if (File.Exists(FilePath))
                    File.Replace(temp, FilePath, null);
                else
                    File.Move(temp, FilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save: não foi possível gravar ({e.Message})");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            data = null;
            database = null;
            Changed = null;
        }
    }
}
