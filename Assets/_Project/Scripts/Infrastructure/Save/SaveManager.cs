using System;
using System.IO;
using DatingSim.Domain;
using UnityEngine;

namespace DatingSim.Infrastructure.Save
{
    /// <summary>
    /// Guardado local en JSON con versionado de esquema.
    /// Solo guarda IDs y estado runtime; nunca assets ni textos.
    /// </summary>
    public class SaveManager
    {
        private readonly string filePath;

        public SaveManager()
        {
            // Calificado: el namespace DatingSim.Application oculta UnityEngine.Application
            filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "save.json");
        }

        public string FilePath => filePath;

        public bool Exists()
        {
            return File.Exists(filePath);
        }

        public void Save(GameState state)
        {
            if (state == null)
            {
                Debug.LogError("[SaveManager] No se puede guardar un estado nulo.");
                return;
            }

            var data = new SaveData
            {
                schemaVersion = SaveData.CurrentSchemaVersion,
                gameState = state
            };

            try
            {
                File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Error al guardar: {e.Message}");
            }
        }

        /// <summary>
        /// Devuelve el estado guardado o null si no existe / está corrupto /
        /// tiene una versión de esquema no soportada.
        /// </summary>
        public GameState LoadOrNull()
        {
            if (!Exists()) return null;

            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<SaveData>(json);

                if (data == null || data.gameState == null)
                {
                    Debug.LogWarning("[SaveManager] Archivo de guardado vacío o inválido.");
                    return null;
                }

                if (data.schemaVersion != SaveData.CurrentSchemaVersion)
                {
                    Debug.LogWarning(
                        $"[SaveManager] Versión de guardado {data.schemaVersion} " +
                        $"distinta a la actual {SaveData.CurrentSchemaVersion}. Se ignora.");
                    return null;
                }

                return data.gameState;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Error al cargar: {e.Message}");
                return null;
            }
        }
    }
}
