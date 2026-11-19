using DatingSim.Application;
using DatingSim.Presentation.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DatingSim.Infrastructure.Save
{
    /// <summary>
    /// Atajos de depuración: F5 guardar, F9 cargar.
    /// Muestra feedback con el Toast para verificar el ciclo en Play Mode.
    /// </summary>
    public class SaveHotkeys : MonoBehaviour
    {
        private void Update()
        {
            var root = GameRoot.Instance;
            if (root == null) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.f5Key.wasPressedThisFrame)
            {
                root.SaveGame();
                ToastView.Current?.Show("Partida guardada.");
            }
            else if (keyboard.f9Key.wasPressedThisFrame)
            {
                if (root.LoadGame())
                    ToastView.Current?.Show("Partida cargada.");
                else
                    ToastView.Current?.Show("No hay partida guardada.");
            }
        }
    }
}
