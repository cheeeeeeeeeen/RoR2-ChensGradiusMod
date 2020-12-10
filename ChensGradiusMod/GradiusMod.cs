#undef DEBUG

using BepInEx;
using BepInEx.Configuration;
using Chen.ClassicItems;
using Chen.GradiusMod.Compatibility;
using Chen.GradiusMod.Drones;
using Chen.Helpers;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.LogHelpers;
using EntityStates;
using KomradeSpectre.Aetherium;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TILER2;
using UnityEngine;
using static Chen.Helpers.GeneralHelpers.AssetsManager;
using static TILER2.MiscUtil;
using EquipmentDroneDeathState = Chen.GradiusMod.Drones.EquipmentDrone.DeathState;
using MegaDroneDeathState = Chen.GradiusMod.Drones.TC280.DeathState;
using Path = System.IO.Path;
using Turret1DeathState = Chen.GradiusMod.Drones.GunnerTurret.DeathState;

[assembly: InternalsVisibleTo("ChensGradiusMod.Tests")]

namespace Chen.GradiusMod
{
    /// <summary>
    /// The mother plugin that allows the mod to be loaded to RoR2.
    /// </summary>
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    [BepInDependency(HelperPlugin.ModGuid, HelperPlugin.ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInDependency(AetheriumPlugin.ModGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ClassicItemsPlugin.ModGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(DirectorAPI))]
    public class GradiusModPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// The version of the mod.
        /// </summary>
        public const string ModVer =
#if DEBUG
            "0." +
#endif
            "2.2.6";

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public const string ModName = "ChensGradiusMod";

        /// <summary>
        /// The GUID of the mod.
        /// </summary>
        public const string ModGuid = "com.Chen.ChensGradiusMod";

        internal static readonly GlobalConfig generalCfg = new GlobalConfig();
        internal static ConfigFile cfgFile;
        internal static FilingDictionary<CatalogBoilerplate> chensItemList = new FilingDictionary<CatalogBoilerplate>();
        internal static List<DroneInfo> gradiusDronesList = new List<DroneInfo>();
        internal static Log Log;

#if DEBUG

        internal static SoundPlayer soundPlayer = new SoundPlayer();

        private void Update()
        {
            DropletGenerator.Update();
            var i9 = Input.GetKeyDown(KeyCode.F9);
            if (i9)
            {
                RoR2.CharacterMaster master = RoR2.PlayerCharacterMasterController.instances[0].master;
                RoR2.CharacterBody body = master.GetBody();
                Transform trans = body.gameObject.transform;
                if (i9)
                {
                    OptionMasterTracker.SpawnOption(trans.gameObject, 1);
                    return;
                }
            }
            soundPlayer.Update();
        }

#endif

        private void Awake()
        {
            Log = new Log(Logger);

#if DEBUG
            MultiplayerTest.Enable(Log);
            soundPlayer.RegisterKeybind(KeyCode.Alpha1, FireLaser.chargeLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha2, FireLaser.dissipateLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha3, GradiusOption.getOptionEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha4, GradiusOption.loseOptionEventId);
#endif

            Log.Debug("Loading assets...");
            BundleInfo models = new BundleInfo("@ChensGradiusMod", "ChensGradiusMod.chensgradiusmod_assets", BundleType.UnityAssetBundle);
            BundleInfo sounds = new BundleInfo("@ChensGradiusMod", "ChensGradiusMod.chensgradiusmod_soundbank.bnk", BundleType.WWiseSoundBank);
            AssetsManager manager = new AssetsManager(models, sounds);
            manager.RegisterAll();

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            Log.Debug("Loading global configs...");
            generalCfg.BindAll(cfgFile, ModName, "General");

            Log.Debug("Instantiating item classes...");
            chensItemList = T2Module.InitAll<CatalogBoilerplate>(new T2Module.ModInfo
            {
                displayName = "Chen's Gradius Mod",
                longIdentifier = "ChensGradiusMod",
                shortIdentifier = "CGM",
                mainConfigFile = cfgFile
            });
            T2Module.SetupAll_PluginAwake(chensItemList);
            T2Module.SetupAll_PluginStart(chensItemList);

            Log.Debug("Instantiating drones...");
            gradiusDronesList = DroneCatalog.Initialize(ModGuid, cfgFile);
            DroneCatalog.EfficientSetupAll(gradiusDronesList);

            Log.Debug("Applying vanilla fixes...");
            RegisterVanillaChanges();

            Log.Debug("Applying compatibility changes...");
            if (Compatibility.Aetherium.enabled) Compatibility.Aetherium.Setup();
            if (ChensClassicItems.enabled) ChensClassicItems.Setup();
        }

        private void RegisterVanillaChanges()
        {
            if (generalCfg.emergencyDroneFix)
            {
                Log.Debug("Vanilla Fix: Applying emergencyDroneFix.");
                On.RoR2.HealBeamController.HealBeamAlreadyExists_GameObject_HealthComponent += HealBeamController_HealBeamAlreadyExists_GO_HC;
            }
            if (generalCfg.turretsAreRepurchaseable)
            {
                Log.Debug("Vanilla Change: Applying turretsAreRepurchaseable.");
                AssignDeathBehavior("prefabs/characterbodies/Turret1Body", typeof(Turret1DeathState));
            }
            if (generalCfg.megaDronesAreRepurchaseable)
            {
                Log.Debug("Vanilla Change: Applying megaDronesAreRepurchaseable.");
                AssignDeathBehavior("prefabs/characterbodies/MegaDroneBody", typeof(MegaDroneDeathState));
            }
            if (generalCfg.dropEquipFromDroneChance > 0f)
            {
                Log.Debug("Vanilla Change: Applying dropEquipFromDroneChance.");
                AssignDeathBehavior("prefabs/characterbodies/EquipmentDroneBody", typeof(EquipmentDroneDeathState));
            }
        }

        private bool HealBeamController_HealBeamAlreadyExists_GO_HC(
            On.RoR2.HealBeamController.orig_HealBeamAlreadyExists_GameObject_HealthComponent orig,
            GameObject owner, HealthComponent targetHealthComponent
        )
        {
            // Note that this is incompatible with other mods. This applies a fix on Emergency Drone. Configs are applied on hook assignment so no need to check here.
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
        }

        internal static bool DebugCheck()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}