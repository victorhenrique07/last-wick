using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Assets._Project.Code.Runtime.Cards;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Code.Editor
{
    // Preenche o Id das cartas sozinho, a partir do nome do arquivo (ChamaCerteira -> chama-certeira).
    // Roda quando uma carta é criada, importada ou duplicada; um Id já preenchido nunca muda,
    // a não ser que repita o de outra carta (caso da carta duplicada com Ctrl+D).
    public class CardIdAssigner : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            foreach (string path in imported)
            {
                if (path.EndsWith(".asset") && AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(CardData))
                {
                    // Espera o fim da importação para não salvar asset no meio dela
                    EditorApplication.delayCall += AssignMissing;
                    return;
                }
            }
        }

        [MenuItem("Last Wick/Preencher IDs das cartas")]
        public static void AssignMissing()
        {
            var cards = new List<(CardData card, string path)>();
            foreach (string guid in AssetDatabase.FindAssets("t:CardData"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                cards.Add((AssetDatabase.LoadAssetAtPath<CardData>(path), path));
            }

            // Ids que ficam: preenchidos, sem repetição; na repetição, fica quem tem o Id do próprio nome de arquivo
            var taken = new HashSet<string>();
            var pending = new List<(CardData card, string path)>();

            foreach (var entry in cards)
                if (!string.IsNullOrWhiteSpace(entry.card.Id) && entry.card.Id == Slug(entry.path))
                    taken.Add(entry.card.Id);

            foreach (var entry in cards)
            {
                string id = entry.card.Id;
                if (string.IsNullOrWhiteSpace(id) || (id != Slug(entry.path) && !taken.Add(id)))
                    pending.Add(entry);
            }

            foreach (var (card, path) in pending)
            {
                string id = Slug(path);
                for (int n = 2; !taken.Add(id); n++)
                    id = $"{Slug(path)}-{n}";

                card.Id = id;
                EditorUtility.SetDirty(card);
                AssetDatabase.SaveAssetIfDirty(card);
                Debug.Log($"Carta {Path.GetFileName(path)} recebeu o Id \"{id}\".", card);
            }
        }

        // "ChamaCerteira 1.asset" -> "chama-certeira-1"
        private static string Slug(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).Normalize(NormalizationForm.FormD);
            var result = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue; // tira acentos

                if (char.IsLetterOrDigit(c))
                {
                    bool newWord = char.IsUpper(c) && i > 0 && char.IsLetterOrDigit(name[i - 1]) && !char.IsUpper(name[i - 1]);
                    if (newWord && result.Length > 0) result.Append('-');
                    result.Append(char.ToLowerInvariant(c));
                }
                else if (result.Length > 0 && result[^1] != '-')
                {
                    result.Append('-');
                }
            }

            return result.ToString().Trim('-');
        }
    }
}
