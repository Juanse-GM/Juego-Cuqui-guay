using DatingSim.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Pantalla de inicio del teléfono: badge de mensajes sin leer
    /// y acceso al chat. Consulta ChatService; no calcula nada por su cuenta.
    /// </summary>
    public class HomeScreen : MonoBehaviour
    {
        [SerializeField] private string characterId = "char_demo";
        [SerializeField] private TextMeshProUGUI unreadBadge;
        [SerializeField] private Button chatButton;
        [SerializeField] private AppNavigationManager navigation;

        private GameRoot root;

        private void Start()
        {
            root = GameRoot.Instance;
            if (root == null || root.Chat == null)
            {
                Debug.LogError("[HomeScreen] GameRoot no encontrado. Ejecuta desde Bootstrap.", this);
                return;
            }

            if (chatButton != null) chatButton.onClick.AddListener(OpenChat);
            root.StateManager.OnStateChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (root != null) root.StateManager.OnStateChanged -= Refresh;
        }

        private void OpenChat()
        {
            if (navigation != null) navigation.ShowChat();
        }

        private void Refresh()
        {
            if (root == null || unreadBadge == null) return;

            int unread = root.Chat.GetUnreadCount(characterId);
            unreadBadge.text = unread > 0 ? unread.ToString() : "";
            unreadBadge.gameObject.SetActive(unread > 0);
        }
    }
}
