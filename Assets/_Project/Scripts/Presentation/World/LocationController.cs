using DatingSim.Application;
using UnityEngine;

namespace DatingSim.Presentation.World
{
    /// <summary>
    /// Registra la localización actual en el GameState al activarse la escena.
    /// </summary>
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private string locationId = "loc_bedroom";

        private void OnEnable()
        {
            var root = GameRoot.Instance;
            if (root == null || root.StateManager == null)
            {
                Debug.LogWarning(
                    "[LocationController] GameRoot no encontrado. " +
                    "Abre el juego desde la escena Bootstrap.", this);
                return;
            }

            root.StateManager.State.SetLocation(locationId);
            root.StateManager.Notify();
        }
    }
}
