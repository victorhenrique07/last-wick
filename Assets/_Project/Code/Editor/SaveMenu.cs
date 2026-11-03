using System.IO;
using Assets._Project.Code.Runtime.Save;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Code.Editor
{
    // Menu para testar o começo do jogo: apaga o save.json (e o .temp que tenha sobrado).
    // O save.corrupt.json fica, para análise.
    public static class SaveMenu
    {
        [MenuItem("Last Wick/Apagar save")]
        private static void DeleteSave()
        {
            // Em Play Mode o save já está na memória; apagar o arquivo agora não muda a partida atual
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Save: saia do Play Mode antes de apagar o save.");
                return;
            }

            bool existed = File.Exists(SaveSystem.FilePath);

            File.Delete(SaveSystem.FilePath);
            File.Delete(SaveSystem.FilePath + ".temp");

            Debug.Log(existed
                ? $"Save: {SaveSystem.FilePath} apagado. O próximo Play começa do zero."
                : $"Save: não havia save em {SaveSystem.FilePath}.");
        }

        [MenuItem("Last Wick/Abrir pasta do save")]
        private static void OpenSaveFolder() => EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }
}
