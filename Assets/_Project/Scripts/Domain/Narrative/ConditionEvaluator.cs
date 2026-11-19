using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Resultado de despachar una interacción (para feedback en UI).
    /// </summary>
    public readonly struct InteractionResult
    {
        public bool Success { get; }
        public string Message { get; }

        private InteractionResult(bool success, string message)
        {
            Success = success;
            Message = message ?? "";
        }

        public static InteractionResult Ok(string message = "") => new InteractionResult(true, message);

        public static InteractionResult Fail(string message) => new InteractionResult(false, message);
    }

    /// <summary>
    /// Evalúa condiciones contra el estado. Devuelve el motivo del fallo
    /// para poder mostrar un feedback coherente al jugador.
    /// </summary>
    public static class ConditionEvaluator
    {
        public static bool Evaluate(List<ConditionData> conditions, GameState state, out string failReason)
        {
            failReason = "";
            if (conditions == null) return true;
            if (state == null)
            {
                failReason = "Estado de partida no disponible.";
                return false;
            }

            for (int i = 0; i < conditions.Count; i++)
            {
                if (!EvaluateOne(conditions[i], state, out failReason))
                    return false;
            }
            return true;
        }

        private static bool EvaluateOne(ConditionData condition, GameState state, out string failReason)
        {
            failReason = "";
            if (condition == null) return true;

            switch (condition.type)
            {
                case ConditionType.FlagSet:
                    if (!state.Flags.Get(condition.key))
                    {
                        failReason = "No disponible ahora.";
                        return false;
                    }
                    return true;

                case ConditionType.FlagNotSet:
                    if (state.Flags.Get(condition.key))
                    {
                        failReason = "Eso ya no está disponible.";
                        return false;
                    }
                    return true;

                case ConditionType.DayAtLeast:
                    if (state.Day < condition.value)
                    {
                        failReason = "Todavía no es el momento.";
                        return false;
                    }
                    return true;

                case ConditionType.MoneyAtLeast:
                    if (state.Money < condition.value)
                    {
                        failReason = "No tengo suficiente dinero.";
                        return false;
                    }
                    return true;

                case ConditionType.AffinityAtLeast:
                {
                    var character = state.GetOrCreateCharacter(condition.key);
                    if (character == null || character.Affinity < condition.value)
                    {
                        failReason = "No tenemos confianza suficiente aún.";
                        return false;
                    }
                    return true;
                }

                default:
                    failReason = "Condición desconocida.";
                    return false;
            }
        }
    }
}
