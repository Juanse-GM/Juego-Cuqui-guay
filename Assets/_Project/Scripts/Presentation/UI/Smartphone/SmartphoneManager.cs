using DatingSim.Application;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Abre/cierra el teléfono. Mientras está abierto, GameRoot
    /// bloquea la interacción con el mundo 3D (WorldPointer lo consulta).
    /// No avanza el tiempo por revisar el teléfono.
    /// Atajos: Tab abre/cierra, Escape cierra.
    /// </summary>
    public class SmartphoneManager : MonoBehaviour
    {
        [SerializeField] private GameObject phonePanel;
        [SerializeField] private AppNavigationManager navigation;
        [Tooltip("Botón opcional en el HUD para abrir el teléfono.")]
        [SerializeField] private UnityEngine.UI.Button phoneButton;

        private GameRoot root;
        public bool IsOpen { get; private set; }

        private void Start()
        {
            root = GameRoot.Instance;
            if (root == null)
                Debug.LogError("[SmartphoneManager] GameRoot no encontrado. Ejecuta desde Bootstrap.", this);
            if (phonePanel != null) phonePanel.SetActive(false);
            if (phoneButton != null) phoneButton.onClick.AddListener(Toggle);
        }

        private void Update()
        {
            if (root == null) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.tabKey.wasPressedThisFrame) Toggle();
            else if (keyboard.escapeKey.wasPressedThisFrame && IsOpen) Close();
        }

        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        public void Open()
        {
            // Primero termina el diálogo: no se puede abrir el teléfono con una conversación activa.
            if (root != null && root.Dialogues != null && root.Dialogues.IsActive) return;

            IsOpen = true;
            if (phonePanel != null) phonePanel.SetActive(true);
            if (navigation != null) navigation.ShowHome();
            if (root != null) root.IsSmartphoneOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
            if (phonePanel != null) phonePanel.SetActive(false);
            if (root != null) root.IsSmartphoneOpen = false;
        }
    }
}
