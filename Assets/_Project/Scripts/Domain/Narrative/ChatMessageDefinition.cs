using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Mensaje de chat: contenido narrativo presentado en el teléfono.
    /// Su visibilidad se evalúa con las mismas condiciones que el mundo 3D.
    /// </summary>
    [Serializable]
    public class ChatMessageDefinition
    {
        public string id;
        public string characterId;
        public string text;
        public List<ConditionData> visibility = new List<ConditionData>();
    }
}
