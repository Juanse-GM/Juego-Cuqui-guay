using System;

namespace DatingSim.Domain
{
    /// <summary>
    /// Estadística de relación que puede modificar un efecto.
    /// </summary>
    public enum RelationshipStat
    {
        Affinity,
        Trust,
        Romance
    }

    /// <summary>
    /// Tipo de efecto de una interacción u opción de diálogo.
    /// Lista controlada: nunca se ejecuta código arbitrario desde datos de contenido.
    /// </summary>
    public enum EffectType
    {
        SetFlag,             // key = flag, lo pone a true
        ClearFlag,           // key = flag, lo elimina
        AddMoney,            // value = cantidad (puede ser negativa)
        ChangeRelationship,  // key = characterId, value = delta, stat = estadística
        ConsumeTime,         // value = nº de bloques horarios
        StartDialogue,       // key = dialogueId
        AddItem              // key = itemId
    }

    /// <summary>
    /// Efecto serializable que se ejecuta contra el GameState vía EffectExecutor.
    /// </summary>
    [Serializable]
    public class EffectData
    {
        public EffectType type;
        public string key = "";
        public int value;
        public RelationshipStat stat = RelationshipStat.Affinity;
    }
}
