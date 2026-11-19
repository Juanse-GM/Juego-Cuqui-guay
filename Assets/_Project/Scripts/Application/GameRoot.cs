using DatingSim.Content;
using DatingSim.Domain;
using DatingSim.Infrastructure.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DatingSim.Application
{
    /// <summary>
    /// Composition root del juego. Inicializa todos los servicios,
    /// los mantiene vivos entre escenas (DontDestroyOnLoad) y carga
    /// la escena de localización. Todo pasa por aquí: ninguna escena
    /// crea sus propias copias de los servicios.
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public const string LocationSceneName = "Location_Bedroom";

        public static GameRoot Instance { get; private set; }

        [Header("Contenido")]
        [SerializeField] private ContentDatabase content;

        public GameStateManager StateManager { get; private set; }
        public TimeManager Time { get; private set; }
        public InteractionDispatcher Interactions { get; private set; }
        public DialogueService Dialogues { get; private set; }
        public ChatService Chat { get; private set; }
        public ContentDatabase Content => content;

        /// <summary>
        /// Lo activa SmartphoneManager para bloquear la interacción con el mundo 3D.
        /// </summary>
        public bool IsSmartphoneOpen { get; set; }

        /// <summary>
        /// El mundo 3D solo recibe clicks cuando esto es false.
        /// </summary>
        public bool BlocksWorldInteraction =>
            IsSmartphoneOpen || (Dialogues != null && Dialogues.IsActive);

        private SaveManager save;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (content == null)
            {
                Debug.LogError("[GameRoot] Falta la referencia a ContentDatabase en el Inspector.");
                enabled = false;
                return;
            }

            save = new SaveManager();

            var state = save.LoadOrNull() ?? new GameState();
            StateManager = new GameStateManager(state);
            Time = new TimeManager(StateManager);

            var effectContext = new GameEffectContext();
            Dialogues = new DialogueService(content, StateManager, effectContext);
            effectContext.Initialize(Time, Dialogues);
            Interactions = new InteractionDispatcher(content, StateManager, effectContext);
            Chat = new ChatService(content, StateManager);

            Debug.Log(save.Exists()
                ? "[GameRoot] Servicios inicializados con partida guardada."
                : "[GameRoot] Servicios inicializados (nueva partida).");
        }

        private void Start()
        {
            SceneManager.LoadScene(LocationSceneName);
        }

        public void SaveGame()
        {
            if (save == null || StateManager == null) return;
            save.Save(StateManager.State);
            Debug.Log("[GameRoot] Partida guardada.");
        }

        public bool LoadGame()
        {
            if (save == null || StateManager == null) return false;
            var loaded = save.LoadOrNull();
            if (loaded == null) return false;
            StateManager.ReplaceState(loaded);
            Debug.Log("[GameRoot] Partida cargada.");
            return true;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
