using System;
using System.Collections.Generic;

namespace DatingSim.Domain
{
    /// <summary>
    /// Almacén de flags booleanos (globales, por personaje, etc.).
    /// Campos públicos: es un DTO serializable (JsonUtility / inspector)
    /// y no debe depender de UnityEngine.
    /// </summary>
    [Serializable]
    public class FlagStore
    {
        [Serializable]
        public class FlagEntry
        {
            public string key;
            public bool value;
        }

        public List<FlagEntry> entries = new List<FlagEntry>();

        public bool Get(string key, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null && entries[i].key == key)
                    return entries[i].value;
            }
            return defaultValue;
        }

        public bool Has(string key) => Get(key, false);

        public void Set(string key, bool value)
        {
            if (string.IsNullOrEmpty(key)) return;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null && entries[i].key == key)
                {
                    entries[i].value = value;
                    return;
                }
            }
            entries.Add(new FlagEntry { key = key, value = value });
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            entries.RemoveAll(e => e != null && e.key == key);
        }
    }
}
