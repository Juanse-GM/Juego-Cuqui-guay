using System;
using DatingSim.Domain;

namespace DatingSim.Application
{
    /// <summary>
    /// Propietario del GameState actual. Las vistas se suscriben a
    /// OnStateChanged para re-renderizarse; nadie muta el estado sin notificar.
    /// </summary>
    public class GameStateManager
    {
        public GameState State { get; private set; }

        public event Action OnStateChanged;

        public GameStateManager(GameState state)
        {
            State = state ?? new GameState();
        }

        /// <summary>
        /// Avisa a todas las vistas de que el estado cambió.
        /// Debe llamarse después de cualquier mutación del GameState.
        /// </summary>
        public void Notify()
        {
            OnStateChanged?.Invoke();
        }

        public void ReplaceState(GameState newState)
        {
            State = newState ?? new GameState();
            Notify();
        }
    }
}
