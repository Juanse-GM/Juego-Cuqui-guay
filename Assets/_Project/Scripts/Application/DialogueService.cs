using System;
using DatingSim.Content;
using DatingSim.Domain;
using UnityEngine;

namespace DatingSim.Application
{
    /// <summary>
    /// Ejecuta grafos de diálogo: abrir, elegir opciones, aplicar efectos
    /// y avanzar/cerrar. La UI solo muestra el nodo actual y envía optionIds.
    /// </summary>
    public class DialogueService
    {
        private readonly ContentDatabase content;
        private readonly GameStateManager stateManager;
        private readonly IEffectContext effectContext;

        public DialogueDefinition ActiveDialogue { get; private set; }
        public DialogueNode CurrentNode { get; private set; }
        public bool IsActive => ActiveDialogue != null && CurrentNode != null;

        /// <summary>
        /// Se dispara al abrir, cambiar de nodo o cerrar el diálogo.
        /// </summary>
        public event Action DialogueChanged;

        public DialogueService(
            ContentDatabase content,
            GameStateManager stateManager,
            IEffectContext effectContext)
        {
            this.content = content;
            this.stateManager = stateManager;
            this.effectContext = effectContext;
        }

        public bool StartDialogue(string dialogueId)
        {
            if (content == null || !content.TryGetDialogue(dialogueId, out var definition))
            {
                Debug.LogError($"[DialogueService] Diálogo no encontrado: '{dialogueId}'");
                return false;
            }

            var startNode = definition.GetStartNode();
            if (startNode == null)
            {
                Debug.LogError($"[DialogueService] Diálogo '{dialogueId}' sin nodos.");
                return false;
            }

            ActiveDialogue = definition;
            CurrentNode = startNode;
            DialogueChanged?.Invoke();
            return true;
        }

        public bool ChooseOption(string optionId)
        {
            if (!IsActive || string.IsNullOrEmpty(optionId)) return false;

            DialogueOption option = null;
            var options = CurrentNode.options;
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i] != null && options[i].id == optionId)
                {
                    option = options[i];
                    break;
                }
            }

            if (option == null)
            {
                Debug.LogError($"[DialogueService] Opción no encontrada: '{optionId}'");
                return false;
            }

            var state = stateManager.State;
            if (!ConditionEvaluator.Evaluate(option.conditions, state, out string failReason))
            {
                Debug.LogWarning($"[DialogueService] Opción '{optionId}' bloqueada: {failReason}");
                return false;
            }

            EffectExecutor.Execute(option.effects, state, effectContext);

            if (string.IsNullOrEmpty(option.nextNodeId))
            {
                CloseInternal();
            }
            else
            {
                var next = ActiveDialogue.FindNode(option.nextNodeId);
                if (next == null)
                {
                    Debug.LogError($"[DialogueService] Nodo destino no encontrado: '{option.nextNodeId}'");
                    CloseInternal();
                }
                else
                {
                    CurrentNode = next;
                }
            }

            DialogueChanged?.Invoke();
            stateManager.Notify();
            return true;
        }

        public void Close()
        {
            if (!IsActive) return;
            CloseInternal();
            DialogueChanged?.Invoke();
        }

        private void CloseInternal()
        {
            ActiveDialogue = null;
            CurrentNode = null;
        }
    }
}
