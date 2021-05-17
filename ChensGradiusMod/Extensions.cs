using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Chen.GradiusMod
{
    /// <summary>
    /// Helpful extensions for objects that will be recurring in the mod.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Safely checks if the dictionary has the key and if they key has an existing object.
        /// </summary>
        /// <param name="dictionary">Supposedly the data dictionary from OptionBehavior</param>
        /// <param name="key">Key to search from the dictionary</param>
        /// <returns>True if the key exists and an object is found. False otherwise.</returns>
        public static bool SafeCheck(this Dictionary<string, UnityObject> dictionary, string key)
        {
            return dictionary.ContainsKey(key) && dictionary[key];
        }
    }
}