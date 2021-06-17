using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
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
        /// <param name="dictionary">Supposedly the data dictionary from OptionBehavior.</param>
        /// <param name="key">Key to search from the dictionary.</param>
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

        /// <summary>
        /// Sets all Skill Drivers of the drone to aim towards the enemy.
        /// </summary>
        /// <param name="masterObject">The CharacterMaster GameObject whose Skill Drivers are being modified.</param>
        public static void SetAllDriversToAimTowardsEnemies(this GameObject masterObject)
        {
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            SetAllDriversToAimTowardsEnemies(skillDrivers);
        }

        /// <summary>
        /// Sets all Skill Drivers within the array to aim towards the enemy.
        /// </summary>
        /// <param name="skillDrivers">An array of Skill Drivers that will be modified.</param>
        public static void SetAllDriversToAimTowardsEnemies(this AISkillDriver[] skillDrivers)
        {
            foreach (var skillDriver in skillDrivers)
            {
                skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            }
        }

        /// <summary>
        /// Assigns the Death Behavior of the CharacterMaster GameObject.
        /// </summary>
        /// <param name="masterObject">CharacterMaster GameObject whose DeathBehavior state is being modified.</param>
        /// <param name="newStateType">The new DeathBehavior state.</param>
        public static void AssignDeathBehavior(this GameObject masterObject, Type newStateType)
        {
            CharacterMaster master = masterObject.GetComponent<CharacterMaster>();
            if (!master)
            {
                Log.Warning("Extensions.AssignDeathBehavior: CharacterMaster component not found!");
                return;
            }
            GameObject droneBody = master.bodyPrefab;
            CharacterDeathBehavior deathBehavior = droneBody.GetOrAddComponent<CharacterDeathBehavior>();
            deathBehavior.deathState = new SerializableEntityStateType(newStateType);
        }
    }
}