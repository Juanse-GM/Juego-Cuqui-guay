namespace DatingSim.Domain
{
    /// <summary>
    /// Bloques horarios del día. El orden define cómo avanza el tiempo.
    /// Madrugada (Dawn) es el último bloque: al avanzar, comienza un día nuevo.
    /// </summary>
    public enum TimeBlock
    {
        Morning = 0,    // mañana
        Afternoon = 1,  // tarde
        Night = 2,      // noche
        Dawn = 3        // madrugada
    }
}
