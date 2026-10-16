using BepInEx.Configuration;
using Chen.GradiusMod.Artifacts.Machines;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static R2API.DirectorAPI;
using static R2API.DirectorAPI.Helpers;
using static RoR2.GenericPickupController;
using DroneDeathState = EntityStates.Drone.DeathState;
using R2APIStage = R2API.DirectorAPI.Stage;
using TurretDeathState = EntityStates.Turret1.DeathState;

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
                InteractableActions += Turret1_InteractableActions;
            }
            Log.Debug("Vanilla Change: Applying repurchase rules on vanilla drones.");
            On.EntityStates.Drone.DeathState.OnImpactServer += DroneDeathState_OnImpactServer;
            On.EntityStates.Turret1.DeathState.OnEnter += TurretDeathState_OnEnter;
        }

        private void DroneDeathState_OnImpactServer(On.EntityStates.Drone.DeathState.orig_OnImpactServer orig, DroneDeathState self, Vector3 contactPoint)
        {
            // Custom drones of this mod and of other mods using the Drone API manage their own rules in Drones.DroneDeathState.
            if (self is Drones.DroneDeathState)
            {
                orig(self, contactPoint);
                return;
            }
            CharacterBody body = self.characterBody;
            if (!IsVanillaDroneRepurchaseable(body)) return;
            bool alreadySpawned = self.spawnedRepairObject;
            orig(self, contactPoint);
            if (!alreadySpawned && self.spawnedRepairObject && GetBodyName(body) == "EquipmentDroneBody") TryDropDroneEquipment(body);
        }

        private void TurretDeathState_OnEnter(On.EntityStates.Turret1.DeathState.orig_OnEnter orig, TurretDeathState self)
        {
            orig(self);
            if (!IsVanillaDroneRepurchaseable(self.characterBody)) self.deadFormToSpawnOnDeath = null;
        }

        private static bool IsVanillaDroneRepurchaseable(CharacterBody body)
        {
            if (!body) return true;
            if (Machines.instance != null && Machines.instance.ShouldSuppressBrokenDrone(body)) return false;
            switch (GetBodyName(body))
            {
                case "Drone1Body": return generalCfg.gunnerDronesAreRepurchaseable;
                case "Turret1Body": return generalCfg.turretsAreRepurchaseable;
                case "Drone2Body": return generalCfg.healingDronesAreRepurchaseable;
                case "EmergencyDroneBody": return generalCfg.emergencyDronesAreRepurchaseable;
                case "FlameDroneBody": return generalCfg.flameDronesAreRepurchaseable;
                case "MissileDroneBody": return generalCfg.missileDronesAreRepurchaseable;
                case "EquipmentDroneBody": return generalCfg.equipmentDronesAreRepurchaseable;
                default: return true;
            }
        }

        private static string GetBodyName(CharacterBody body) => body ? BodyCatalog.GetBodyName(body.bodyIndex) : null;

        private static void TryDropDroneEquipment(CharacterBody body)
        {
            if (!NetworkServer.active || !body.inventory || !body.master) return;
            EquipmentIndex equipmentIndex = body.inventory.currentEquipmentIndex;
            if (equipmentIndex == EquipmentIndex.None || !Util.CheckRoll(generalCfg.dropEquipFromDroneChance, body.master)) return;
            CreatePickup(new CreatePickupInfo
            {
                position = body.corePosition,
                rotation = Quaternion.identity,
                pickup = new UniquePickup(PickupCatalog.FindPickupIndex(equipmentIndex))
            });
        }

        private void Turret1_InteractableActions(DccsPool arg1, StageInfo arg2)
        {
            if (!arg1) return;

            ForEachPoolEntryInDccsPool(arg1, (poolEntry) =>
            {
                DirectorCard turretDirectorCard = null;
                poolEntry.dccs.RemoveCardsThatFailFilter(dCard =>
                {
                    if (dCard.spawnCard && dCard.spawnCard.name.Contains("Turret1"))
                    {
                        turretDirectorCard = dCard;
                        return false;
                    }
                    return true;
                });
                if (turretDirectorCard != null)
                {
                    DirectorCard newDirectorCard = new DirectorCard
                    {
                        spawnCard = turretDirectorCard.spawnCard,
                        selectionWeight = turretDirectorCard.selectionWeight,
                        minimumStageCompletions = turretDirectorCard.minimumStageCompletions,
                        spawnDistance = turretDirectorCard.spawnDistance,
                        preventOverhead = turretDirectorCard.preventOverhead
                    };
                    DirectorCardHolder newDirectorCardHolder = new DirectorCardHolder
                    {
                        Card = newDirectorCard,
                        MonsterCategory = MonsterCategory.Invalid,
                        InteractableCategory = InteractableCategory.Drones,
                    };
                    poolEntry.dccs.AddCard(newDirectorCardHolder);
                }
            });
        }

        private void FlameDrone_InteractableActions(DccsPool arg1, StageInfo arg2)
        {
            if (!arg1) return;
            float weightMultiplier;
            if (arg2.CheckStage(R2APIStage.AbyssalDepths)) weightMultiplier = generalCfg.flameDroneWeightAbyssalDepths;
            else if (arg2.CheckStage(R2APIStage.ScorchedAcres)) weightMultiplier = generalCfg.flameDroneWeightScorchedAcres;
            else return;
            if (weightMultiplier <= 1f) return;

            ForEachPoolEntryInDccsPool(arg1, (poolEntry) =>
            {
                DirectorCard flameDroneDirectorCard = null;
                poolEntry.dccs.RemoveCardsThatFailFilter(dCard =>
                {
                    if (dCard.spawnCard && dCard.spawnCard.name.Contains("FlameDrone"))
                    {
                        flameDroneDirectorCard = dCard;
                        return false;
                    }
                    return true;
                });
                if (flameDroneDirectorCard != null)
                {
                    DirectorCard newDirectorCard = new DirectorCard
                    {
                        spawnCard = flameDroneDirectorCard.spawnCard,
                        selectionWeight = Mathf.RoundToInt(flameDroneDirectorCard.selectionWeight * weightMultiplier),
                        minimumStageCompletions = flameDroneDirectorCard.minimumStageCompletions,
                        spawnDistance = flameDroneDirectorCard.spawnDistance,
                        preventOverhead = flameDroneDirectorCard.preventOverhead
                    };
                    DirectorCardHolder newDirectorCardHolder = new DirectorCardHolder
                    {
                        Card = newDirectorCard,
                        MonsterCategory = MonsterCategory.Invalid,
                        InteractableCategory = InteractableCategory.Drones,
                    };
                    poolEntry.dccs.AddCard(newDirectorCardHolder);
                }
            });
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

        internal class GlobalConfig
        {
            private const string Section = "General";

            public bool emergencyDroneFix { get; private set; } = true;
            public bool gunnerDronesAreRepurchaseable { get; private set; } = true;
            public bool turretsAreRepurchaseable { get; private set; } = true;
            public bool healingDronesAreRepurchaseable { get; private set; } = true;
            public bool emergencyDronesAreRepurchaseable { get; private set; } = true;
            public bool flameDronesAreRepurchaseable { get; private set; } = true;
            public bool missileDronesAreRepurchaseable { get; private set; } = true;
            public bool megaDronesAreRepurchaseable { get; private set; } = true;
            public bool equipmentDronesAreRepurchaseable { get; private set; } = true;
            public float dropEquipFromDroneChance { get; private set; } = 0f;
            public int flameDroneWeightScorchedAcres { get; private set; } = 1;
            public int flameDroneWeightAbyssalDepths { get; private set; } = 3;
            public bool turretsAsDroneCategory { get; private set; } = true;

            // The keys match the ones TILER2 generated so that existing config files keep their values.
            public void Bind(ConfigFile config)
            {
                emergencyDroneFix = config.Bind(Section, "EmergencyDroneFix", emergencyDroneFix,
                    "Applies a fix for Emergency Drones. Set to false if there are issues regarding compatibility.").Value;

                gunnerDronesAreRepurchaseable = BindRepurchase(config, "GunnerDronesAreRepurchaseable", "Gunner Drones", gunnerDronesAreRepurchaseable);
                turretsAreRepurchaseable = BindRepurchase(config, "TurretsAreRepurchaseable", "Gunner Turrets", turretsAreRepurchaseable);
                healingDronesAreRepurchaseable = BindRepurchase(config, "HealingDronesAreRepurchaseable", "Healing Drones", healingDronesAreRepurchaseable);
                emergencyDronesAreRepurchaseable = BindRepurchase(config, "EmergencyDronesAreRepurchaseable", "Emergency Drones", emergencyDronesAreRepurchaseable);
                flameDronesAreRepurchaseable = BindRepurchase(config, "FlameDronesAreRepurchaseable", "Incinerator Drones", flameDronesAreRepurchaseable);
                missileDronesAreRepurchaseable = BindRepurchase(config, "MissileDronesAreRepurchaseable", "Missile Drones", missileDronesAreRepurchaseable);
                megaDronesAreRepurchaseable = BindRepurchase(config, "MegaDronesAreRepurchaseable", "TC-280 Prototypes", megaDronesAreRepurchaseable);
                equipmentDronesAreRepurchaseable = BindRepurchase(config, "EquipmentDronesAreRepurchaseable", "Equipment Drones", equipmentDronesAreRepurchaseable);

                dropEquipFromDroneChance = config.Bind(Section, "DropEquipFromDroneChance", dropEquipFromDroneChance, new ConfigDescription(
                    "Allows Equipment Drones to have a chance to drop their Equipment when destroyed. Value of 43 means 43% chance. Affected by the drone's luck.",
                    new AcceptableValueRange<float>(0f, 100f))).Value;

                flameDroneWeightScorchedAcres = config.Bind(Section, "FlameDroneWeightScorchedAcres", flameDroneWeightScorchedAcres, new ConfigDescription(
                    "Flame Drone spawn weight multiplier in Scorched Acres. Set to 1 for default.", new AcceptableValueRange<int>(1, int.MaxValue))).Value;

                flameDroneWeightAbyssalDepths = config.Bind(Section, "FlameDroneWeightAbyssalDepths", flameDroneWeightAbyssalDepths, new ConfigDescription(
                    "Flame Drone spawn weight multiplier in Abyssal Depths. Set to 1 for default.", new AcceptableValueRange<int>(1, int.MaxValue))).Value;

                turretsAsDroneCategory = config.Bind(Section, "TurretsAsDroneCategory", turretsAsDroneCategory,
                    "Gunner Turrets are not treated as Drones by the Director and they fall under Miscellaneous Category, hence why gunner turrets spawn more often. " +
                    "Setting this to true will change the category of turrets to Drones as well.").Value;
            }

            private static bool BindRepurchase(ConfigFile config, string key, string droneName, bool defaultValue)
            {
                return config.Bind(Section, key, defaultValue,
                    $"Allows all {droneName} to be repurchased when they are destroyed or decommissioned. " +
                    "Drones owned by enemies or given by the Artifact of Machines are never repurchaseable.").Value;
            }
        }
    }
}
