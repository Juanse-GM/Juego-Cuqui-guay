using TMPro;
using UnityEngine;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Tooltip flotante de los hotspots (Screen Space Overlay).
    /// Single runtime: lo encuentran las demás vistas sin referencias
    /// entre escenas. El componente va en un objeto siempre activo;
    /// solo se activa/desactiva el panel hijo "tipRoot".
    /// </summary>
    public class TooltipView : MonoBehaviour
    {
        public static TooltipView Current { get; private set; }

        [SerializeField] private RectTransform tipRoot;
        [SerializeField] private TextMeshProUGUI label;

        private void OnEnable()
        {
            Current = this;
        }

        private void OnDisable()
        {
            if (Current == this) Current = null;
        }

        public void Show(string text, Vector2 screenPosition)
        {
            if (tipRoot == null || label == null) return;
            label.text = text;
            tipRoot.position = screenPosition;
            if (!tipRoot.gameObject.activeSelf)
                tipRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (tipRoot != null && tipRoot.gameObject.activeSelf)
                tipRoot.gameObject.SetActive(false);
        }
    }
}
