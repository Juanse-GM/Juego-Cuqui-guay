using System.Collections;
using TMPro;
using UnityEngine;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Mensaje temporal (feedback de interacciones, guardado, etc.).
    /// Single runtime como TooltipView: componente en objeto siempre activo,
    /// el panel hijo se activa/desactiva.
    /// </summary>
    public class ToastView : MonoBehaviour
    {
        public static ToastView Current { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private float duration = 2.5f;

        private Coroutine routine;

        private void OnEnable()
        {
            Current = this;
        }

        private void OnDisable()
        {
            if (Current == this) Current = null;
        }

        public void Show(string message)
        {
            if (panel == null || label == null || string.IsNullOrEmpty(message)) return;

            label.text = message;
            panel.SetActive(true);

            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(duration);
            panel.SetActive(false);
            routine = null;
        }
    }
}
