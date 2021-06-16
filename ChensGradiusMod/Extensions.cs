using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
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

        /// <summary>
        /// Method that provides an easy way of displaying effect prefabs for muzzle effects.
        /// Mainly used for Options and Option Seeds.
        /// </summary>
        /// <param name="catalyst">The Game Object of the user</param>
        /// <param name="effectPrefab">Effect prefab to display</param>
        /// <param name="transmit">Determines whether the effect should be networked</param>
        public static void MuzzleEffect(this GameObject catalyst, GameObject effectPrefab, bool transmit)
        {
            EffectData data = new EffectData
            {
                origin = catalyst.transform.position,
                rotation = catalyst.transform.rotation,
                rootObject = catalyst
            };
            EffectManager.SpawnEffect(effectPrefab, data, transmit);
        }

        internal static void SetAllDriversToAimTowardsEnemies(this GameObject masterObject)
        {
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            SetAllDriversToAimTowardsEnemies(skillDrivers);
        }

        internal static void SetAllDriversToAimTowardsEnemies(this AISkillDriver[] skillDrivers)
        {
            foreach (var skillDriver in skillDrivers)
            {
                skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            }
        }
    }
}