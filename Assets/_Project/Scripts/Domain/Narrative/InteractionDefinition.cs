using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Tipo de condición de una interacción, mensaje o opción de diálogo.
    /// Lista controlada (sin código arbitrario desde datos de contenido).
    /// </summary>
    public enum ConditionType
    {
        FlagSet,         // key = nombre del flag, debe ser true
        FlagNotSet,      // key = nombre del flag, debe ser false/inexistente
        DayAtLeast,      // value = día mínimo
        MoneyAtLeast,    // value = dinero mínimo
        AffinityAtLeast  // key = characterId, value = afinidad mínima
    }

    /// <summary>
    /// Condición serializable evaluada contra el GameState.
    /// </summary>
    [Serializable]
    public class ConditionData
    {
        public ConditionType type;
        public string key = "";
        public int value;
    }

    /// <summary>
    /// Política de repetición de una interacción.
    /// </summary>
    public enum RepeatPolicy
    {
        Once,       // una sola vez por partida
        Repeatable, // siempre disponible (si las condiciones se cumplen)
        Daily       // una vez por día
    }

    /// <summary>
    /// Definición de una interacción de hotspot: qué pasa al seleccionarla.
    /// Solo datos: la validación y ejecución las hacen ConditionEvaluator y EffectExecutor.
    /// </summary>
    [Serializable]
    public class InteractionDefinition
    {
        public string id;
        public string presentationText = "";
        public RepeatPolicy repeatPolicy = RepeatPolicy.Repeatable;
        public List<ConditionData> conditions = new List<ConditionData>();
        public List<EffectData> effects = new List<EffectData>();
    }
}
