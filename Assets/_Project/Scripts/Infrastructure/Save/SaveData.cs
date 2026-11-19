using System;
using DatingSim.Domain;

namespace DatingSim.Infrastructure.Save
{
    /// <summary>
    /// Estructura del archivo de guardado. schemaVersion permite
    /// migrar partidas cuando cambie el modelo en el futuro.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public const int CurrentSchemaVersion = 1;

        public int schemaVersion = CurrentSchemaVersion;
        public GameState gameState;
    }
}
