#undef DEBUG

using Aetherium;
using BepInEx;
using BepInEx.Configuration;
using Chen.ClassicItems;
using Chen.GradiusMod.Compatibility;
using Chen.GradiusMod.Drones;
using Chen.Helpers;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.LogHelpers;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TILER2;
using UnityEngine;
using static Chen.Helpers.GeneralHelpers.AssetsManager;
using static TILER2.MiscUtil;
using EmergencyDroneDeathState = Chen.GradiusMod.Drones.EmergencyDrone.DeathState;
using EquipmentDroneDeathState = Chen.GradiusMod.Drones.EquipmentDrone.DeathState;
using GunnerDroneDeathState = Chen.GradiusMod.Drones.GunnerDrone.DeathState;
using GunnerTurretDeathState = Chen.GradiusMod.Drones.GunnerTurret.DeathState;
using HealingDroneDeathState = Chen.GradiusMod.Drones.HealingDrone.DeathState;
using IncineratorDroneDeathState = Chen.GradiusMod.Drones.IncineratorDrone.DeathState;
using MissileDroneDeathState = Chen.GradiusMod.Drones.MissileDrone.DeathState;
using Path = System.IO.Path;
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
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(DirectorAPI), nameof(ContentAddition))]
    public partial class GradiusModPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// The version of the mod.
        /// </summary>
        public const string ModVer = "3.5.4";

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

        private static readonly Lazy<GameObject> _backupDroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/DroneBackupMaster"));
        private static readonly Lazy<GameObject> _drone1Master = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/Drone1Master"));
        private static readonly Lazy<GameObject> _drone2Master = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/Drone2Master"));
        private static readonly Lazy<GameObject> _emergencyDroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/EmergencyDroneMaster"));
        private static readonly Lazy<GameObject> _flameDroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/FlameDroneMaster"));
        private static readonly Lazy<GameObject> _missileDroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/DroneMissileMaster"));
        private static readonly Lazy<GameObject> _turret1Master = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/Turret1Master"));
        private static readonly Lazy<GameObject> _tc280DroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster"));
        private static readonly Lazy<GameObject> _equipmentDroneMaster = new Lazy<GameObject>(() => Resources.Load<GameObject>("prefabs/charactermasters/EquipmentDroneMaster"));
        private static readonly Lazy<GameObject> _helperPrefab = new Lazy<GameObject>(() => Resources.Load<GameObject>("SpawnCards/HelperPrefab"));
        private static readonly Lazy<InteractableSpawnCard> _drone1SpawnCard = new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1"));
        private static readonly Lazy<SkillDef> _drone1Skill = new Lazy<SkillDef>(() => Resources.Load<SkillDef>("skilldefs/drone1body/Drone1BodyGun"));
        private static readonly Lazy<Material> _summonDroneMaterial = new Lazy<Material>(() => Resources.Load<Material>("Materials/matSummonDrone"));

        internal static GameObject backupDroneMaster { get => _backupDroneMaster.Value; }
        internal static GameObject drone1Master { get => _drone1Master.Value; }
        internal static GameObject drone2Master { get => _drone2Master.Value; }
        internal static GameObject emergencyDroneMaster { get => _emergencyDroneMaster.Value; }
        internal static GameObject flameDroneMaster { get => _flameDroneMaster.Value; }
        internal static GameObject missileDroneMaster { get => _missileDroneMaster.Value; }
        internal static GameObject turret1Master { get => _turret1Master.Value; }
        internal static GameObject tc280DroneMaster { get => _tc280DroneMaster.Value; }
        internal static GameObject equipmentDroneMaster { get => _equipmentDroneMaster.Value; }
        internal static GameObject helperPrefab { get => _helperPrefab.Value; }
        internal static InteractableSpawnCard drone1SpawnCard { get => _drone1SpawnCard.Value; }
        internal static SkillDef drone1Skill { get => _drone1Skill.Value; }
        internal static Material summonDroneMaterial { get => _summonDroneMaterial.Value; }

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
                    Items.GradiusOption.Components.OptionMasterTracker.SpawnOption(trans.gameObject, 1);
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
            soundPlayer.RegisterKeybind(KeyCode.Alpha1, Drones.LaserDrone.FireLaser.ChargeLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha2, Drones.LaserDrone.FireLaser.DissipateLaserEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha3, Items.GradiusOption.GradiusOption.GetOptionEventId);
            soundPlayer.RegisterKeybind(KeyCode.Alpha4, Items.GradiusOption.GradiusOption.LoseOptionEventId);
#endif
            Log.Debug("Loading assets...");
            BundleInfo models = new BundleInfo("Chen.GradiusMod.chensgradiusmod_assets", BundleType.UnityAssetBundle);
            BundleInfo sounds = new BundleInfo("Chen.GradiusMod.chensgradiusmod_soundbank.bnk", BundleType.WWiseSoundBank);
            AssetsManager modelsManager = new AssetsManager(models);
            AssetsManager soundsManager = new AssetsManager(sounds);
            assetBundle = modelsManager.Register();
            assetBundle.ConvertShaders();
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
            equipmentDroneMaster.SetAllDriversToAimTowardsEnemies();
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