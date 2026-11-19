using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Fuente de verdad del estado de una partida.
    /// DTO serializable sin dependencia de UnityEngine (JsonUtility lo serializa tal cual).
    /// Las vistas NUNCA modifican esto directamente: lo hacen los servicios de aplicación
    /// mediante efectos/dispatcher, que luego notifican con GameStateManager.Notify().
    /// </summary>
    [Serializable]
    public class GameState
    {
        public const int StartingMoney = 200;

        public int day = 1;
        public TimeBlock timeBlock = TimeBlock.Morning;
        public int money = StartingMoney;
        public string locationId = "";
        public FlagStore flags = new FlagStore();
        public List<CharacterRuntimeState> characterStates = new List<CharacterRuntimeState>();
        public List<string> inventory = new List<string>();

        public int Day => day;
        public TimeBlock CurrentTimeBlock => timeBlock;
        public int Money => money;
        public string LocationId => locationId;
        public FlagStore Flags => flags;
        public IReadOnlyList<CharacterRuntimeState> CharacterStates => characterStates;
        public IReadOnlyList<string> Inventory => inventory;

        public void SetLocation(string id)
        {
            locationId = id ?? "";
        }

        public void SetTimeBlock(TimeBlock block)
        {
            timeBlock = block;
        }

        /// <summary>
        /// Avanza un bloque horario. Al pasar de madrugada, inicia un día nuevo.
        /// </summary>
        public void AdvanceTimeBlock()
        {
            if (timeBlock == TimeBlock.Dawn)
            {
                timeBlock = TimeBlock.Morning;
                day++;
            }
            else
            {
                timeBlock = (TimeBlock)((int)timeBlock + 1);
            }
        }

        public void AddMoney(int delta)
        {
            money += delta;
            if (money < 0) money = 0;
        }

        public CharacterRuntimeState GetOrCreateCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId)) return null;
            for (int i = 0; i < characterStates.Count; i++)
            {
                if (characterStates[i] != null && characterStates[i].characterId == characterId)
                    return characterStates[i];
            }
            var created = new CharacterRuntimeState(characterId);
            characterStates.Add(created);
            return created;
        }

        public CharacterRuntimeState GetCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId)) return null;
            for (int i = 0; i < characterStates.Count; i++)
            {
                if (characterStates[i] != null && characterStates[i].characterId == characterId)
                    return characterStates[i];
            }
            return null;
        }

        public void AddItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            if (!inventory.Contains(itemId)) inventory.Add(itemId);
        }

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && inventory.Contains(itemId);
        }
    }
}
