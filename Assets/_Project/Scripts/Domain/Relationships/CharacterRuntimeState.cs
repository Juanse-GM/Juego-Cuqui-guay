using System;

namespace DatingSim.Domain
{
    /// <summary>
    /// Progreso mutable de un personaje en la partida actual.
    /// Nunca se guarda en el Animator: solo aquí y en el save.
    /// Campos públicos por ser DTO serializable sin dependencia de UnityEngine.
    /// </summary>
    [Serializable]
    public class CharacterRuntimeState
    {
        public const int StatMin = 0;
        public const int StatMax = 100;

        public string characterId;
        public int affinity;
        public int trust;
        public int romance;

        public int Affinity => affinity;
        public int Trust => trust;
        public int Romance => romance;

        public CharacterRuntimeState()
        {
        }

        public CharacterRuntimeState(string characterId)
        {
            this.characterId = characterId;
        }

        public int GetStat(RelationshipStat stat)
        {
            switch (stat)
            {
                case RelationshipStat.Trust: return trust;
                case RelationshipStat.Romance: return romance;
                default: return affinity;
            }
        }

        public void SetStat(RelationshipStat stat, int value)
        {
            int clamped = Clamp(value);
            switch (stat)
            {
                case RelationshipStat.Trust: trust = clamped; break;
                case RelationshipStat.Romance: romance = clamped; break;
                default: affinity = clamped; break;
            }
        }

        private static int Clamp(int value)
        {
            if (value < StatMin) return StatMin;
            if (value > StatMax) return StatMax;
            return value;
        }
    }
}
