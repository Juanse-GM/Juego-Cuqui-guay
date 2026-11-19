using System.Collections.Generic;
using DatingSim.Content;
using DatingSim.Domain;

namespace DatingSim.Application
{
    /// <summary>
    /// Consulta de mensajes de chat: visibilidad (condiciones) y no leídos.
    /// La UI del teléfono llama aquí; no calcula nada por su cuenta.
    /// </summary>
    public class ChatService
    {
        private readonly ContentDatabase content;
        private readonly GameStateManager stateManager;

        public ChatService(ContentDatabase content, GameStateManager stateManager)
        {
            this.content = content;
            this.stateManager = stateManager;
        }

        public List<ChatMessageDefinition> GetVisibleMessages(string characterId)
        {
            var result = new List<ChatMessageDefinition>();
            if (content == null || string.IsNullOrEmpty(characterId)) return result;

            var state = stateManager.State;
            var messages = content.Messages;
            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                if (message == null || message.characterId != characterId) continue;
                if (ConditionEvaluator.Evaluate(message.visibility, state, out _))
                    result.Add(message);
            }
            return result;
        }

        public int GetUnreadCount(string characterId)
        {
            int unread = 0;
            var visible = GetVisibleMessages(characterId);
            for (int i = 0; i < visible.Count; i++)
            {
                if (!stateManager.State.Flags.Get(GetReadFlag(visible[i])))
                    unread++;
            }
            return unread;
        }

        public void MarkAllRead(string characterId)
        {
            bool changed = false;
            var state = stateManager.State;
            var visible = GetVisibleMessages(characterId);
            for (int i = 0; i < visible.Count; i++)
            {
                string flag = GetReadFlag(visible[i]);
                if (!state.Flags.Get(flag))
                {
                    state.Flags.Set(flag, true);
                    changed = true;
                }
            }
            if (changed) stateManager.Notify();
        }

        private static string GetReadFlag(ChatMessageDefinition message)
        {
            return "read_" + message.id;
        }
    }
}
