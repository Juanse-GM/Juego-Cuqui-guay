using DatingSim.Application;
using DatingSim.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Panel de diálogo del mundo 3D. Muestra el nodo actual del
    /// DialogueService; los botones solo envían optionIds, sin lógica.
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI speakerLabel;
        [SerializeField] private TextMeshProUGUI bodyLabel;
        [SerializeField] private RectTransform optionsContainer;
        [Tooltip("Botón de opción inactivo, hijo del contenedor. Se clona por cada opción.")]
        [SerializeField] private Button optionTemplate;
        [SerializeField] private Button closeButton;

        private DialogueService dialogues;
        private ContentDatabase content;

        private void Start()
        {
            var root = GameRoot.Instance;
            if (root == null || root.Dialogues == null)
            {
                Debug.LogError("[DialoguePanel] GameRoot no encontrado. Ejecuta desde Bootstrap.", this);
                return;
            }

            dialogues = root.Dialogues;
            content = root.Content;

            dialogues.DialogueChanged += Refresh;
            if (closeButton != null)
                closeButton.onClick.AddListener(() => dialogues.Close());

            Refresh();
        }

        private void OnDestroy()
        {
            if (dialogues != null) dialogues.DialogueChanged -= Refresh;
        }

        private void Refresh()
        {
            if (dialogues == null || panelRoot == null) return;

            bool active = dialogues.IsActive;
            panelRoot.SetActive(active);
            if (!active) return;

            var node = dialogues.CurrentNode;
            if (node == null) return;

            if (speakerLabel != null)
                speakerLabel.text = content != null
                    ? content.GetCharacterName(node.speakerId)
                    : node.speakerId;
            if (bodyLabel != null)
                bodyLabel.text = node.text;

            ClearOptions();
            if (node.options == null || optionTemplate == null) return;

            for (int i = 0; i < node.options.Count; i++)
            {
                var option = node.options[i];
                if (option == null) continue;

                var button = Instantiate(optionTemplate, optionsContainer);
                button.gameObject.SetActive(true);

                var label = button.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = option.text;

                string optionId = option.id;
                button.onClick.AddListener(() => OnOptionChosen(optionId));
            }
        }

        private void OnOptionChosen(string optionId)
        {
            if (dialogues == null) return;
            if (!dialogues.ChooseOption(optionId))
                ToastView.Current?.Show("No puedo decir eso ahora.");
        }

        private void ClearOptions()
        {
            if (optionsContainer == null || optionTemplate == null) return;

            for (int i = optionsContainer.childCount - 1; i >= 0; i--)
            {
                var child = optionsContainer.GetChild(i);
                if (child.gameObject == optionTemplate.gameObject) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
