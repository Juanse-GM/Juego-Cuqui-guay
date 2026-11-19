using DatingSim.Application;
using DatingSim.Presentation.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DatingSim.Presentation.World
{
    /// <summary>
    /// Entrada Point & Click del mundo 3D:
    /// mouse → guard de UI → Physics.Raycast → Hotspot → dispatcher.
    /// No contiene lógica narrativa: solo detecta y delega.
    /// </summary>
    public class WorldPointer : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private LayerMask interactableMask = ~0;
        [SerializeField] private InteractionFeedback feedback;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private Vector2 tooltipOffset = new Vector2(18f, -18f);

        private Hotspot hovered;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = Camera.main;
            if (viewCamera == null)
                Debug.LogError("[WorldPointer] No hay cámara asignada ni etiquetada MainCamera.", this);
            if (feedback == null)
                Debug.LogError("[WorldPointer] Falta la referencia a InteractionFeedback.", this);
        }

        private void Update()
        {
            var root = GameRoot.Instance;
            var mouse = Mouse.current;

            // Guardas: sin servicios, teléfono/diálogo abierto, puntero sobre UI
            if (root == null || mouse == null || viewCamera == null ||
                root.BlocksWorldInteraction ||
                (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
            {
                ClearPointer();
                return;
            }

            Vector2 screenPos = mouse.position.ReadValue();
            var ray = viewCamera.ScreenPointToRay(screenPos);

            Hotspot hitHotspot = null;
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableMask))
                hitHotspot = hit.collider.GetComponentInParent<Hotspot>();

            SetHover(hitHotspot);

            if (hitHotspot != null)
            {
                TooltipView.Current?.Show(hitHotspot.DisplayName, screenPos + tooltipOffset);

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    var result = hitHotspot.TryInteract();
                    if (!string.IsNullOrEmpty(result.Message))
                        ToastView.Current?.Show(result.Message);
                }
            }
            else
            {
                TooltipView.Current?.Hide();
            }
        }

        private void SetHover(Hotspot hotspot)
        {
            if (hovered == hotspot) return;
            hovered = hotspot;
            if (feedback != null) feedback.SetHover(hotspot);
        }

        private void ClearPointer()
        {
            SetHover(null);
            TooltipView.Current?.Hide();
        }

        private void OnDisable()
        {
            ClearPointer();
        }
    }
}
