using System.Collections.Generic;
using DatingSim.Application;
using DatingSim.Domain;
using TMPro;
using UnityEngine;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Pantalla de chat: lista los mensajes visibles según el GameState.
    /// Al mostrarse marca los mensajes como leídos (vía ChatService).
    /// </summary>
    public class ChatScreen : MonoBehaviour
    {
        [SerializeField] private string characterId = "char_demo";
        [SerializeField] private RectTransform messageContainer;
        [Tooltip("GameObject inactivo con TextMeshProUGUI; se clona por cada mensaje.")]
        [SerializeField] private GameObject messageTemplate;

        private GameRoot root;

        private void OnEnable()
        {
            // Se ejecuta cada vez que la pantalla se muestra.
            MarkReadAndRefresh();
        }

        private void Start()
        {
            root = GameRoot.Instance;
            if (root == null || root.Chat == null)
            {
                Debug.LogError("[ChatScreen] GameRoot no encontrado. Ejecuta desde Bootstrap.", this);
                return;
            }

            root.StateManager.OnStateChanged += Refresh;
            MarkReadAndRefresh();
        }

        private void OnDestroy()
        {
            if (root != null) root.StateManager.OnStateChanged -= Refresh;
        }

        private void MarkReadAndRefresh()
        {
            if (root == null) return;
            root.Chat.MarkAllRead(characterId);
            Refresh();
        }

        private void Refresh()
        {
            if (root == null || messageContainer == null || messageTemplate == null) return;

            ClearRows();

            List<ChatMessageDefinition> messages = root.Chat.GetVisibleMessages(characterId);
            string characterName = root.Content != null
                ? root.Content.GetCharacterName(characterId)
                : characterId;

            for (int i = 0; i < messages.Count; i++)
            {
                var row = Instantiate(messageTemplate, messageContainer);
                row.SetActive(true);

                var label = row.GetComponent<TextMeshProUGUI>();
                if (label != null)
                    label.text = $"{characterName}: {messages[i].text}";
            }
        }

        private void ClearRows()
        {
            for (int i = messageContainer.childCount - 1; i >= 0; i--)
            {
                var child = messageContainer.GetChild(i);
                if (child.gameObject == messageTemplate) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
