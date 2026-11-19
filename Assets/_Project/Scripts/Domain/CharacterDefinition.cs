using System;

namespace DatingSim.Domain
{
    /// <summary>
    /// Datos estáticos mínimos de un personaje (definición de contenido).
    /// Se llama CharacterDefinition para evitar la colisión con UnityEngine.CharacterInfo.
    /// </summary>
    [Serializable]
    public class CharacterDefinition
    {
        public string id;
        public string displayName;
    }
}
