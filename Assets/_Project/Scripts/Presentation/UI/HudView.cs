using DatingSim.Application;
using DatingSim.Domain;
using TMPro;
using UnityEngine;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// HUD: muestra día, bloque horario y dinero.
    /// Solo lee el GameState; nunca lo modifica.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayLabel;
        [SerializeField] private TextMeshProUGUI timeLabel;
        [SerializeField] private TextMeshProUGUI moneyLabel;

        private GameStateManager stateManager;

        private void Start()
        {
            var root = GameRoot.Instance;
            if (root == null || root.StateManager == null)
            {
                Debug.LogError("[HudView] GameRoot no encontrado. Ejecuta desde Bootstrap.", this);
                return;
            }

            stateManager = root.StateManager;
            stateManager.OnStateChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (stateManager != null) stateManager.OnStateChanged -= Refresh;
        }

        private void Refresh()
        {
            var state = stateManager.State;
            if (dayLabel != null) dayLabel.text = $"Día {state.Day}";
            if (timeLabel != null) timeLabel.text = GetTimeBlockName(state.CurrentTimeBlock);
            if (moneyLabel != null) moneyLabel.text = $"{state.Money} $";
        }

        private static string GetTimeBlockName(TimeBlock block)
        {
            switch (block)
            {
                case TimeBlock.Morning: return "Mañana";
                case TimeBlock.Afternoon: return "Tarde";
                case TimeBlock.Night: return "Noche";
                case TimeBlock.Dawn: return "Madrugada";
                default: return block.ToString();
            }
        }
    }
}
