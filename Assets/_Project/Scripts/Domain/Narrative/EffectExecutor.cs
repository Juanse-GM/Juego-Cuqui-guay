using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Acciones que los efectos pueden pedir fuera del GameState puro
    /// (avanzar el tiempo, abrir un diálogo). Lo implementa la capa de aplicación.
    /// </summary>
    public interface IEffectContext
    {
        void ConsumeTime(int blocks);
        void StartDialogue(string dialogueId);
    }

    /// <summary>
    /// Ejecuta efectos sobre el GameState. Es la única vía por la que
    /// el contenido (interacciones/opciones) modifica el estado.
    /// </summary>
    public static class EffectExecutor
    {
        public static void Execute(List<EffectData> effects, GameState state, IEffectContext context)
        {
            if (effects == null || state == null) return;
            for (int i = 0; i < effects.Count; i++)
            {
                ExecuteOne(effects[i], state, context);
            }
        }

        private static void ExecuteOne(EffectData effect, GameState state, IEffectContext context)
        {
            if (effect == null) return;

            switch (effect.type)
            {
                case EffectType.SetFlag:
                    state.Flags.Set(effect.key, true);
                    break;

                case EffectType.ClearFlag:
                    state.Flags.Remove(effect.key);
                    break;

                case EffectType.AddMoney:
                    state.AddMoney(effect.value);
                    break;

                case EffectType.ChangeRelationship:
                {
                    var character = state.GetOrCreateCharacter(effect.key);
                    if (character != null)
                    {
                        int current = character.GetStat(effect.stat);
                        character.SetStat(effect.stat, current + effect.value);
                    }
                    break;
                }

                case EffectType.ConsumeTime:
                    context?.ConsumeTime(effect.value);
                    break;

                case EffectType.StartDialogue:
                    context?.StartDialogue(effect.key);
                    break;

                case EffectType.AddItem:
                    state.AddItem(effect.key);
                    break;

                default:
                    throw new InvalidOperationException($"EffectType desconocido: {effect.type}");
            }
        }
    }
}
