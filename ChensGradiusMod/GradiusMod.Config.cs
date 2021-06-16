#undef DEBUG

using Chen.Helpers.LogHelpers;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TILER2;
using UnityEngine;
using static R2API.DirectorAPI;
using EquipmentDroneDeathState = Chen.GradiusMod.Drones.EquipmentDrone.DeathState;
using MegaDroneDeathState = Chen.GradiusMod.Drones.TC280.DeathState;
using R2APIStage = R2API.DirectorAPI.Stage;
using Turret1DeathState = Chen.GradiusMod.Drones.GunnerTurret.DeathState;

[assembly: InternalsVisibleTo("ChensGradiusMod.Tests")]

namespace Chen.GradiusMod
{
    public partial class GradiusModPlugin
    {
        internal static readonly GlobalConfig generalCfg = new GlobalConfig();

        private void RegisterVanillaChanges()
        {
            if (generalCfg.emergencyDroneFix)
            {
                Log.Debug("Vanilla Fix: Applying Emergency Drone Fix.");
                On.RoR2.HealBeamController.HealBeamAlreadyExists_GameObject_HealthComponent += HealBeamController_HealBeamAlreadyExists_GO_HC;
            }
            if (generalCfg.turretsAreRepurchaseable)
            {
                Log.Debug("Vanilla Change: Applying Repurchasable Turrets.");
                AssignDeathBehavior("prefabs/characterbodies/Turret1Body", typeof(Turret1DeathState));
            }
            if (generalCfg.megaDronesAreRepurchaseable)
            {
                Log.Debug("Vanilla Change: Applying Repurchasable TC-280 Prototypes.");
                AssignDeathBehavior("prefabs/characterbodies/MegaDroneBody", typeof(MegaDroneDeathState));
            }
            if (generalCfg.dropEquipFromDroneChance > 0f)
            {
                Log.Debug("Vanilla Change: Adding chance to drop Equipment from Equipment Drones.");
                AssignDeathBehavior("prefabs/characterbodies/EquipmentDroneBody", typeof(EquipmentDroneDeathState));
            }
            if (generalCfg.flameDroneWeightScorchedAcres > 0 || generalCfg.flameDroneWeightAbyssalDepths > 0)
            {
                Log.Debug("Vanilla Change: Modifying Flame Drone spawn weight on Abyssal Depths or Scorched Acres (or both).");
                InteractableActions += FlameDrone_InteractableActions;
            }
            if (generalCfg.smarterVanillaDrones)
            {
                Log.Debug("Vanilla Change: Modifying vanilla drone behavior to be smarter.");
                ModifyVanillaDronesSkillDrivers();
            }
        }

        private void FlameDrone_InteractableActions(List<DirectorCardHolder> arg1, StageInfo arg2)
        {
            DirectorCard dcFlameDrone = null;
            foreach (DirectorCardHolder dch in arg1)
            {
                if (dcFlameDrone == null && dch.Card.spawnCard.name.Contains("FlameDrone"))
                {
                    dcFlameDrone = dch.Card;
                    break;
                }
            }
            if (dcFlameDrone != null)
            {
                if (generalCfg.flameDroneWeightAbyssalDepths > 0 && arg2.stage == R2APIStage.AbyssalDepths)
                {
                    dcFlameDrone.selectionWeight *= generalCfg.flameDroneWeightAbyssalDepths;
                }
                else if (generalCfg.flameDroneWeightScorchedAcres > 0 && arg2.stage == R2APIStage.ScorchedAcres)
                {
                    dcFlameDrone.selectionWeight *= generalCfg.flameDroneWeightScorchedAcres;
                }
            }
        }

        private bool HealBeamController_HealBeamAlreadyExists_GO_HC(
            On.RoR2.HealBeamController.orig_HealBeamAlreadyExists_GameObject_HealthComponent orig,
            GameObject owner, HealthComponent targetHealthComponent
        )
        {
            // Note that this is incompatible with other mods. This applies a fix on Emergency Drone.
            // Configs are applied on hook assignment so no need to check here.
            List<HealBeamController> instancesList = InstanceTracker.GetInstancesList<HealBeamController>();
            for (int i = 0; i < instancesList.Count; i++)
            {
                HealBeamController hbc = instancesList[i];
                if (!hbc || !hbc.target || !hbc.target.healthComponent || !targetHealthComponent || !hbc.ownership || !hbc.ownership.ownerObject)
                {
                    continue;
                }
                if (hbc.target.healthComponent == targetHealthComponent && hbc.ownership.ownerObject == owner)
                {
                    return true;
                }
            }
            return false;
        }

        private void AssignDeathBehavior(string prefabPath, Type newStateType)
        {
            GameObject droneBody = Resources.Load<GameObject>(prefabPath);
            CharacterDeathBehavior deathBehavior = droneBody.GetComponent<CharacterDeathBehavior>();
            deathBehavior.deathState = new SerializableEntityStateType(newStateType);
        }

        private void ModifyVanillaDronesSkillDrivers()
        {
            drone1Master.SetAllDriversToAimTowardsEnemies();
            tc280DroneMaster.SetAllDriversToAimTowardsEnemies();
            missileDroneMaster.SetAllDriversToAimTowardsEnemies();
            backupDroneMaster.SetAllDriversToAimTowardsEnemies();
            flameDroneMaster.SetAllDriversToAimTowardsEnemies();
        }

        internal class GlobalConfig : AutoConfigContainer
        {
            [AutoConfig("Applies a fix for Emergency Drones. Set to false if there are issues regarding compatibility.", AutoConfigFlags.PreventNetMismatch)]
            public bool emergencyDroneFix { get; private set; } = true;

            [AutoConfig("Allows all Gunner Turrets to be repurchased when they are destroyed or decommissioned.", AutoConfigFlags.PreventNetMismatch)]
            public bool turretsAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all TC-280 Prototypes to be repurchased when they are destroyed or decommissioned.", AutoConfigFlags.PreventNetMismatch)]
            public bool megaDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows Equipment Drones to have a chance to drop their Equipment when destroyed. 43 = 43% chance. Affected by the drone's luck. " +
                        "0 will disable this.", AutoConfigFlags.PreventNetMismatch, 0f, 100f)]
            public float dropEquipFromDroneChance { get; private set; } = 0f;

            [AutoConfig("Aetherium Compatibility: Allow Equipment Drones to be Inspired by Inspiring Drone.", AutoConfigFlags.PreventNetMismatch)]
            public bool equipmentDroneInspire { get; private set; } = true;

            [AutoConfig("Flame Drone spawn weight multiplier in Scorched Acres. Set to 0 for default.", AutoConfigFlags.PreventNetMismatch, 0, int.MaxValue)]
            public int flameDroneWeightScorchedAcres { get; private set; } = 0;

            [AutoConfig("Flame Drone spawn weight multiplier in Abyssal Depths. Set to 0 for default.", AutoConfigFlags.PreventNetMismatch, 0, int.MaxValue)]
            public int flameDroneWeightAbyssalDepths { get; private set; } = 3;

            [AutoConfig("Modifies the vanilla Drone A.I. to be smarter. For example, this change makes Gunner Drones not attack the players.", AutoConfigFlags.PreventNetMismatch)]
            public bool smarterVanillaDrones { get; private set; } = true;
        }
    }
}