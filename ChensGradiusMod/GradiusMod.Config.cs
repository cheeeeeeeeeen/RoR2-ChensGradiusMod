#undef DEBUG

using Chen.Helpers.LogHelpers;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TILER2;
using UnityEngine;
using static R2API.DirectorAPI;
using R2APIStage = R2API.DirectorAPI.Stage;

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
            if (generalCfg.flameDroneWeightScorchedAcres > 1 || generalCfg.flameDroneWeightAbyssalDepths > 1)
            {
                Log.Debug("Vanilla Change: Modifying Flame Drone spawn weight on Abyssal Depths or Scorched Acres (or both).");
                InteractableActions += FlameDrone_InteractableActions;
            }
            if (generalCfg.turretsAsDroneCategory)
            {
                Log.Debug("Vanilla Change: Modifying category of Gunner Turrets to Drones category.");
                InteractableActions += GradiusModPlugin_InteractableActions;
            }
        }

        private void GradiusModPlugin_InteractableActions(List<DirectorCardHolder> arg1, StageInfo arg2)
        {
            List<DirectorCardHolder> cardHolders = arg1.FindAll(item =>
            {
                return item.InteractableCategory == InteractableCategory.Misc &&
                       item.Card.spawnCard == turret1SpawnCard;
            });
            foreach (var cardHolder in cardHolders)
            {
                cardHolder.InteractableCategory = InteractableCategory.Drones;
                cardHolder.Card.selectionWeight = 1;
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
                if (generalCfg.flameDroneWeightAbyssalDepths > 1 && arg2.stage == R2APIStage.AbyssalDepths)
                {
                    dcFlameDrone.selectionWeight *= generalCfg.flameDroneWeightAbyssalDepths;
                }
                else if (generalCfg.flameDroneWeightScorchedAcres > 1 && arg2.stage == R2APIStage.ScorchedAcres)
                {
                    dcFlameDrone.selectionWeight *= generalCfg.flameDroneWeightScorchedAcres;
                }
            }
            else Log.Warning("GradiusModPlugin.FlameDrone_InteractableActions: Flame Drone Director Card not found!");
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

        internal class GlobalConfig : AutoConfigContainer
        {
            [AutoConfig("Applies a fix for Emergency Drones. Set to false if there are issues regarding compatibility.", AutoConfigFlags.PreventNetMismatch)]
            public bool emergencyDroneFix { get; private set; } = true;

            [AutoConfig("Allows all Gunner Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool gunnerDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Gunner Turrets to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool turretsAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Healing Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool healingDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Emergency Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool emergencyDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Incinerator Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool flameDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Missile Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool missileDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all TC-280 Prototypes to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool megaDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows all Equipment Drones to be repurchased when they are destroyed or decommissioned. " +
                        "This config only affects the base value, and can be changed by external sources like Artifact of Machines.",
                        AutoConfigFlags.PreventNetMismatch)]
            public bool equipmentDronesAreRepurchaseable { get; private set; } = true;

            [AutoConfig("Allows Equipment Drones to have a chance to drop their Equipment when destroyed. Value of 43 means 43% chance. Affected by the drone's luck.",
                        AutoConfigFlags.PreventNetMismatch, 0f, 100f)]
            public float dropEquipFromDroneChance { get; private set; } = 0f;

            [AutoConfig("Flame Drone spawn weight multiplier in Scorched Acres. Set to 1 for default.", AutoConfigFlags.PreventNetMismatch, 1, int.MaxValue)]
            public int flameDroneWeightScorchedAcres { get; private set; } = 1;

            [AutoConfig("Flame Drone spawn weight multiplier in Abyssal Depths. Set to 1 for default.", AutoConfigFlags.PreventNetMismatch, 1, int.MaxValue)]
            public int flameDroneWeightAbyssalDepths { get; private set; } = 3;

            [AutoConfig("Gunner Turrets are not treated as Drones by the Director and they fall under Miscellaneous Category, hence why gunner turrets spawn more often. " +
                        "Setting this to true will change the category of turrets to Drones as well.", AutoConfigFlags.PreventNetMismatch)]
            public bool turretsAsDroneCategory { get; private set; } = true;

            [AutoConfig("Aetherium Compatibility: Allow Equipment Drones to be Inspired by Inspiring Drone.", AutoConfigFlags.PreventNetMismatch)]
            public bool equipmentDroneInspire { get; private set; } = true;
        }
    }
}