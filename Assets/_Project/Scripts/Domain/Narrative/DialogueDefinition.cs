using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Grafo de conversación: nodos con opciones. Solo datos;
    /// la ejecución la hace DialogueService en la capa de aplicación.
    /// </summary>
    [Serializable]
    public class DialogueDefinition
    {
        public string id;
        public string startNodeId = "";
        public List<DialogueNode> nodes = new List<DialogueNode>();

        public DialogueNode FindNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null && nodes[i].id == nodeId)
                    return nodes[i];
            }
            return null;
        }

        public DialogueNode GetStartNode()
        {
            if (!string.IsNullOrEmpty(startNodeId))
            {
                var node = FindNode(startNodeId);
                if (node != null) return node;
            }
            return nodes.Count > 0 ? nodes[0] : null;
        }
    }

    [Serializable]
    public class DialogueNode
    {
        public string id;
        public string speakerId;
        public string text;
        public List<DialogueOption> options = new List<DialogueOption>();
    }

    [Serializable]
    public class DialogueOption
    {
        public string id;
        public string text;
        public string nextNodeId = ""; // vacío = cierra la conversación
        public List<ConditionData> conditions = new List<ConditionData>();
        public List<EffectData> effects = new List<EffectData>();
    }
}
