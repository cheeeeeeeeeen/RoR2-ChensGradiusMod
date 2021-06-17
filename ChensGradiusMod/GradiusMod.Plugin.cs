#define DEBUG

using Aetherium;
using BepInEx;
using BepInEx.Configuration;
using Chen.ClassicItems;
using Chen.GradiusMod.Compatibility;
using Chen.GradiusMod.Drones;
using Chen.GradiusMod.Drones.LaserDrone;
using Chen.GradiusMod.Items.GradiusOption;
using Chen.GradiusMod.Items.GradiusOption.Components;
using Chen.Helpers;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.LogHelpers;
using EntityStates;
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
using GunnerDroneDeathState = Chen.GradiusMod.Drones.GunnerDrone.DeathState;
using Path = System.IO.Path;
using GunnerTurretDeathState = Chen.GradiusMod.Drones.GunnerTurret.DeathState;
using EmergencyDroneDeathState = Chen.GradiusMod.Drones.EmergencyDrone.DeathState;
using EquipmentDroneDeathState = Chen.GradiusMod.Drones.EquipmentDrone.DeathState;
using HealingDroneDeathState = Chen.GradiusMod.Drones.HealingDrone.DeathState;
using IncineratorDroneDeathState = Chen.GradiusMod.Drones.IncineratorDrone.DeathState;
using MissileDroneDeathState = Chen.GradiusMod.Drones.MissileDrone.DeathState;
using TC280DeathState = Chen.GradiusMod.Drones.TC280.DeathState;

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
    public partial class GradiusModPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// The version of the mod.
        /// </summary>
        public const string ModVer =
#if DEBUG
            "0." +
#endif
            "3.3.5";

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public const string ModName = "ChensGradiusMod";

        /// <summary>
        /// The GUID of the mod.
        /// </summary>
        public const string ModGuid = "com.Chen.ChensGradiusMod";

        internal static ConfigFile cfgFile;
        internal static FilingDictionary<CatalogBoilerplate> chensItemList = new FilingDictionary<CatalogBoilerplate>();
        internal static List<DroneInfo> gradiusDronesList = new List<DroneInfo>();
        internal static Log Log;
        internal static AssetBundle assetBundle;
        internal static ContentProvider contentProvider;

        internal static GameObject backupDroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/DroneBackupMaster"); }
        internal static GameObject drone1Master { get => Resources.Load<GameObject>("prefabs/charactermasters/Drone1Master"); }
        internal static GameObject drone2Master { get => Resources.Load<GameObject>("prefabs/charactermasters/Drone2Master"); }
        internal static GameObject emergencyDroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/EmergencyDroneMaster"); }
        internal static GameObject flameDroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/FlameDroneMaster"); }
        internal static GameObject missileDroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/DroneMissileMaster"); }
        internal static GameObject turret1Master { get => Resources.Load<GameObject>("prefabs/charactermasters/Turret1Master"); }
        internal static GameObject tc280DroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster"); }
        internal static GameObject equipmentDroneMaster { get => Resources.Load<GameObject>("prefabs/charactermasters/EquipmentDroneMaster"); }
        internal static GameObject helperPrefab { get => Resources.Load<GameObject>("SpawnCards/HelperPrefab"); }

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
            contentProvider = new ContentProvider();

#if DEBUG
            MultiplayerTest.Enable(Log);
            soundPlayer.RegisterKeybind(KeyCode.Alpha1, FireLaser.chargeLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha2, FireLaser.dissipateLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha3, GradiusOption.getOptionEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha4, GradiusOption.loseOptionEventId);
#endif

            Log.Debug("Loading assets...");
            BundleInfo models = new BundleInfo("Chen.GradiusMod.chensgradiusmod_assets", BundleType.UnityAssetBundle);
            BundleInfo sounds = new BundleInfo("Chen.GradiusMod.chensgradiusmod_soundbank.bnk", BundleType.WWiseSoundBank);
            AssetsManager modelsManager = new AssetsManager(models);
            AssetsManager soundsManager = new AssetsManager(sounds);
            assetBundle = modelsManager.Register() as AssetBundle;
            soundsManager.Register();

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            Log.Debug("Loading global configs...");
            generalCfg.BindAll(cfgFile, ModName, "General");

            Log.Debug("Modifying vanilla drone behavior...");
            ModifyVanillaDroneDeathBehaviors();
            ModifyVanillaDronesSkillDrivers();

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

            Log.Debug("Instantiating custom drones...");
            gradiusDronesList = DroneCatalog.Initialize(ModGuid, cfgFile);
            DroneCatalog.EfficientSetupAll(gradiusDronesList);

            contentProvider.Initialize();

            Log.Debug("Applying vanilla changes and fixes...");
            RegisterVanillaChanges();

            Log.Debug("Applying compatibility changes...");
            if (Compatibility.Aetherium.enabled) Compatibility.Aetherium.Setup();
            if (ChensClassicItems.enabled) ChensClassicItems.Setup();
        }

        private void ModifyVanillaDroneDeathBehaviors()
        {
            emergencyDroneMaster.AssignDeathBehavior(typeof(EmergencyDroneDeathState));
            equipmentDroneMaster.AssignDeathBehavior(typeof(EquipmentDroneDeathState));
            drone1Master.AssignDeathBehavior(typeof(GunnerDroneDeathState));
            turret1Master.AssignDeathBehavior(typeof(GunnerTurretDeathState));
            drone2Master.AssignDeathBehavior(typeof(HealingDroneDeathState));
            flameDroneMaster.AssignDeathBehavior(typeof(IncineratorDroneDeathState));
            missileDroneMaster.AssignDeathBehavior(typeof(MissileDroneDeathState));
            tc280DroneMaster.AssignDeathBehavior(typeof(TC280DeathState));
        }

        private void ModifyVanillaDronesSkillDrivers()
        {
            drone1Master.SetAllDriversToAimTowardsEnemies();
            tc280DroneMaster.SetAllDriversToAimTowardsEnemies();
            missileDroneMaster.SetAllDriversToAimTowardsEnemies();
            backupDroneMaster.SetAllDriversToAimTowardsEnemies();
            flameDroneMaster.SetAllDriversToAimTowardsEnemies();
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