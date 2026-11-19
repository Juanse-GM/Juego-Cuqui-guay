using DatingSim.Domain;

namespace DatingSim.Application
{
    /// <summary>
    /// Implementación de IEffectContext: conecta los efectos del dominio
    /// con los servicios de aplicación (tiempo y diálogos).
    /// Se inicializa en dos fases para romper la dependencia circular
    /// con DialogueService (que a su vez necesita el context para sus efectos).
    /// </summary>
    public class GameEffectContext : IEffectContext
    {
        private TimeManager time;
        private DialogueService dialogues;

        public void Initialize(TimeManager timeManager, DialogueService dialogueService)
        {
            time = timeManager;
            dialogues = dialogueService;
        }

        public void ConsumeTime(int blocks)
        {
            time?.ConsumeBlocks(blocks);
        }

        public void StartDialogue(string dialogueId)
        {
            dialogues?.StartDialogue(dialogueId);
        }
    }
}
