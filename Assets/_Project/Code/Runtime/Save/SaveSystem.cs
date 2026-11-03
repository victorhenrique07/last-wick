using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            saveData.Deck = saveData.Deck.Where(card => saveData.UnlockedCards.Contains(card)).ToList();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            data = null;
            database = null;
        }

        private static SaveData MoveToCorruptAndStartOver(string reason)
        {
            if (File.Exists(CorruptPath))
                File.Delete(CorruptPath);

            File.Move(FilePath, CorruptPath);
            Debug.LogWarning($"Save data is corrupt: {reason}. The save file has been moved to {CorruptPath}.");

            return SaveData.CreateNew(Database);
        }
    }
}
