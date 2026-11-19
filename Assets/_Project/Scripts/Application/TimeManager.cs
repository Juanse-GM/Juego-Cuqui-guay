using DatingSim.Domain;

namespace DatingSim.Application
{
    /// <summary>
    /// Controla el avance de día y bloques horarios.
    /// Solo las actividades e interacciones marcadas consumen tiempo
    /// (se activan con el efecto ConsumeTime en los datos).
    /// </summary>
    public class TimeManager
    {
        private readonly GameStateManager stateManager;

        public TimeManager(GameStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public void ConsumeBlocks(int blocks)
        {
            if (blocks <= 0) return;
            var state = stateManager.State;
            for (int i = 0; i < blocks; i++)
            {
                state.AdvanceTimeBlock();
            }
            stateManager.Notify();
        }
    }
}
