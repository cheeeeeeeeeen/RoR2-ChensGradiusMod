using Chen.Helpers.RoR2Helpers;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static RoR2.BulletAttack;
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

        /// <summary>
        /// Filters the owner out from the attack so that they do not hit themselves with their own attack.
        /// Useful for Option Seeds' behavior to avoid hitting the owner.
        /// </summary>
        /// <param name="bulletAttack">The bullet attack being worked on.</param>
        public static void FilterOutOwnerFromAttack(this BulletAttack bulletAttack)
        {
            bulletAttack.filterCallback = (ref BulletHit hitInfo) =>
            {
                if (bulletAttack.owner == hitInfo.entityObject || bulletAttack.weapon == hitInfo.entityObject) return false;

                return bulletAttack.DefaultFilterCallback(ref hitInfo);
            };

            bulletAttack.hitCallback = (ref BulletHit hitInfo) =>
            {
                if (bulletAttack.owner == hitInfo.entityObject || bulletAttack.weapon == hitInfo.entityObject) return false;

                return bulletAttack.DefaultHitCallback(ref hitInfo);
            };
        }

        /// <summary>
        /// Shortcut for initializing a custom drone model. Only applies when work flow is followed the same as this mod's drones.
        /// </summary>
        /// <param name="droneModel">The custom drone model to initialize.</param>
        /// <param name="droneBody">The associated body of the model.</param>
        public static void InitializeDroneModelComponents(this GameObject droneModel, CharacterBody droneBody)
        {
            CapsuleCollider hurtBoxCapsuleCollider = droneModel.GetComponentInChildren<CapsuleCollider>();
            if (!hurtBoxCapsuleCollider)
            {
                Log.Warning("Extensions.InitializeDroneModelComponents: CapsuleCollider not found! Cannot setup HurtBoxes without it.");
                return;
            }
            CharacterModel characterModel = droneModel.AddComponent<CharacterModel>();
            characterModel.body = droneBody;
            characterModel.BuildRendererInfos(droneModel);
            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();
            HurtBoxGroup hurtBoxGroup = droneModel.AddComponent<HurtBoxGroup>();
            HurtBox hurtBox = hurtBoxCapsuleCollider.gameObject.AddComponent<HurtBox>();
            hurtBox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            hurtBox.healthComponent = droneBody.GetComponent<HealthComponent>();
            hurtBox.isBullseye = true;
            hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            hurtBox.hurtBoxGroup = hurtBoxGroup;
            hurtBox.indexInGroup = 0;
            hurtBoxGroup.hurtBoxes = new HurtBox[] { hurtBox };
            hurtBoxGroup.mainHurtBox = hurtBox;
            hurtBoxGroup.bullseyeCount = 1;
            CapsuleCollider capsuleCollider = droneBody.GetComponent<CapsuleCollider>();
            if (capsuleCollider)
            {
                capsuleCollider.center = hurtBoxCapsuleCollider.center;
                capsuleCollider.radius = hurtBoxCapsuleCollider.radius;
                capsuleCollider.height = hurtBoxCapsuleCollider.height;
                capsuleCollider.direction = hurtBoxCapsuleCollider.direction;
            }
        }
    }
}