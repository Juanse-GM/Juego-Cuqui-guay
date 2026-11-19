using DatingSim.Content;
using DatingSim.Domain;
using UnityEngine;

namespace DatingSim.Application
{
    /// <summary>
    /// Centro de la interacción: recibe el ID de una interacción,
    /// valida repetición + condiciones, ejecuta efectos y notifica.
    /// Toda la lógica de "qué pasa al hacer click" vive aquí,
    /// nunca en los Hotspots ni en botones de UI.
    /// </summary>
    public class InteractionDispatcher
    {
        private readonly ContentDatabase content;
        private readonly GameStateManager stateManager;
        private readonly IEffectContext effectContext;

        public InteractionDispatcher(
            ContentDatabase content,
            GameStateManager stateManager,
            IEffectContext effectContext)
        {
            this.content = content;
            this.stateManager = stateManager;
            this.effectContext = effectContext;
        }

        public InteractionResult Dispatch(string interactionId)
        {
            if (string.IsNullOrEmpty(interactionId))
                return InteractionResult.Fail("Interacción sin ID.");

            if (content == null || !content.TryGetInteraction(interactionId, out var definition))
            {
                Debug.LogError($"[InteractionDispatcher] Interacción no encontrada: '{interactionId}'");
                return InteractionResult.Fail("Interacción desconocida.");
            }

            var state = stateManager.State;

            // Política de repetición
            string repeatFlag = GetRepeatFlag(definition, state);
            if (repeatFlag != null && state.Flags.Get(repeatFlag))
            {
                return InteractionResult.Fail(
                    definition.repeatPolicy == RepeatPolicy.Daily
                        ? "Ya lo he hecho hoy."
                        : "Ya lo he hecho antes.");
            }

            // Condiciones
            if (!ConditionEvaluator.Evaluate(definition.conditions, state, out string failReason))
                return InteractionResult.Fail(failReason);

            // Efectos
            EffectExecutor.Execute(definition.effects, state, effectContext);

            // Registrar cumplimiento de la política de repetición
            if (repeatFlag != null)
                state.Flags.Set(repeatFlag, true);

            stateManager.Notify();
            return InteractionResult.Ok(definition.presentationText);
        }

        private static string GetRepeatFlag(InteractionDefinition definition, GameState state)
        {
            switch (definition.repeatPolicy)
            {
                case RepeatPolicy.Once:
                    return "done_" + definition.id;
                case RepeatPolicy.Daily:
                    return "daily_" + definition.id + "_d" + state.Day;
                default:
                    return null;
            }
        }
    }
}
