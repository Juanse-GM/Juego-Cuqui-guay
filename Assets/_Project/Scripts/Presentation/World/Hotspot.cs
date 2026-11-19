using DatingSim.Application;
using DatingSim.Domain;
using UnityEngine;

namespace DatingSim.Presentation.World
{
    /// <summary>
    /// Hotspot delgado: solo identifica la interacción que representa.
    /// Sin if/else narrativos aquí: las condiciones y efectos viven en
    /// InteractionDefinition y los ejecuta InteractionDispatcher.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hotspot : MonoBehaviour
    {
        [Header("Identidad (no usar nombres de GameObject como ID)")]
        [SerializeField] private string hotspotId = "";
        [Tooltip("ID de la interacción en ContentDatabase.")]
        [SerializeField] private string interactionDefinitionId = "";
        [Tooltip("Texto visible en el tooltip. Si está vacío se usa hotspotId.")]
        [SerializeField] private string displayName = "";

        public string HotspotId => hotspotId;
        public string InteractionDefinitionId => interactionDefinitionId;
        public string DisplayName =>
            string.IsNullOrEmpty(displayName) ? hotspotId : displayName;

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
                Debug.LogWarning($"[Hotspot] '{name}' necesita un Collider.", this);
            if (string.IsNullOrEmpty(hotspotId))
                Debug.LogWarning($"[Hotspot] '{name}' sin hotspotId.", this);
            if (string.IsNullOrEmpty(interactionDefinitionId))
                Debug.LogWarning($"[Hotspot] '{name}' sin interactionDefinitionId.", this);
        }

        /// <summary>
        /// Delega la lógica al dispatcher central. Devuelve el resultado
        /// para que WorldPointer muestre el feedback (toast).
        /// </summary>
        public InteractionResult TryInteract()
        {
            var root = GameRoot.Instance;
            if (root == null || root.Interactions == null)
            {
                return InteractionResult.Fail(
                    "GameRoot no encontrado. Ejecuta el juego desde la escena Bootstrap.");
            }
            return root.Interactions.Dispatch(interactionDefinitionId);
        }
    }
}
