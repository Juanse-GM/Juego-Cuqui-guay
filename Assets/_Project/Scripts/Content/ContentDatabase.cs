using System.Collections.Generic;
using DatingSim.Domain;
using UnityEngine;

namespace DatingSim.Content
{
    /// <summary>
    /// Base de datos de contenido del juego: definiciones editables
    /// desde el Inspector. Las pantallas NUNCA guardan su propia copia
    /// de este contenido: lo consultan a travÃ©s de los servicios.
    /// </summary>
    [CreateAssetMenu(fileName = "ContentDatabase", menuName = "Dating Sim/Content Database")]
    public class ContentDatabase : ScriptableObject
    {
        [Header("Interacciones (hotspots)")]
        [SerializeField] private List<InteractionDefinition> interactions = new List<InteractionDefinition>();

        [Header("DiÃ¡logos")]
        [SerializeField] private List<DialogueDefinition> dialogues = new List<DialogueDefinition>();

        [Header("Mensajes de chat")]
        [SerializeField] private List<ChatMessageDefinition> messages = new List<ChatMessageDefinition>();

        [Header("Personajes")]
        [SerializeField] private List<CharacterDefinition> characters = new List<CharacterDefinition>();

        public IReadOnlyList<InteractionDefinition> Interactions => interactions;
        public IReadOnlyList<DialogueDefinition> Dialogues => dialogues;
        public IReadOnlyList<ChatMessageDefinition> Messages => messages;
        public IReadOnlyList<CharacterDefinition> Characters => characters;

        public bool TryGetInteraction(string id, out InteractionDefinition definition)
        {
            definition = FindById(interactions, d => d?.id, id);
            return definition != null;
        }

        public bool TryGetDialogue(string id, out DialogueDefinition definition)
        {
            definition = FindById(dialogues, d => d?.id, id);
            return definition != null;
        }

        public CharacterDefinition GetCharacter(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i].id == id)
                    return characters[i];
            }
            return null;
        }

        public string GetCharacterName(string id)
        {
            var character = GetCharacter(id);
            if (character == null || string.IsNullOrEmpty(character.displayName)) return id;
            return character.displayName;
        }

        private static T FindById<T>(List<T> list, System.Func<T, string> idSelector, string id)
            where T : class
        {
            if (string.IsNullOrEmpty(id) || list == null) return null;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item != null && idSelector(item) == id) return item;
            }
            return null;
        }
    }
}
