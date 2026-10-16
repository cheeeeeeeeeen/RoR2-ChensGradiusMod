using BepInEx;
using BepInEx.Configuration;
using Chen.GradiusMod.Artifacts.Machines;
using Chen.GradiusMod.Drones;
using Chen.GradiusMod.Items.GradiusOption;
using Chen.GradiusMod.Items.OptionSeed;
using Chen.GradiusMod.Modules;
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
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Chen.Helpers.GeneralHelpers.AssetsManager;
using Path = System.IO.Path;

[assembly: InternalsVisibleTo("ChensGradiusMod.Tests")]

namespace Chen.GradiusMod
{
    /// <summary>
    /// The mother plugin that allows the mod to be loaded to RoR2.
    /// </summary>
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(HelperPlugin.ModGuid, HelperPlugin.ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public partial class GradiusModPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// The version of the mod.
        /// </summary>
        public const string ModVer = "4.0.0";

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public const string ModName = "ChensGradiusMod";

        /// <summary>
        /// The GUID of the mod.
        /// </summary>
        public const string ModGuid = "com.Chen.ChensGradiusMod";

        internal static ConfigFile cfgFile;
        internal static List<ModuleBase> chensItemList = new List<ModuleBase>();
        internal static List<Drones.DroneInfo> gradiusDronesList = new List<Drones.DroneInfo>();
        internal static Log Log;
        internal static AssetBundle assetBundle;

        private static readonly Lazy<GameObject> _backupDroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/DroneBackupMaster.prefab");
        private static readonly Lazy<GameObject> _drone1Master = LazyAddressable<GameObject>("RoR2/Base/Drones/Drone1Master.prefab");
        private static readonly Lazy<GameObject> _drone2Master = LazyAddressable<GameObject>("RoR2/Base/Drones/Drone2Master.prefab");
        private static readonly Lazy<GameObject> _emergencyDroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/EmergencyDroneMaster.prefab");
        private static readonly Lazy<GameObject> _flameDroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/FlameDroneMaster.prefab");
        private static readonly Lazy<GameObject> _missileDroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/DroneMissileMaster.prefab");
        private static readonly Lazy<GameObject> _turret1Master = LazyAddressable<GameObject>("RoR2/Base/Drones/Turret1Master.prefab");
        private static readonly Lazy<GameObject> _tc280DroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/MegaDroneMaster.prefab");
        private static readonly Lazy<GameObject> _equipmentDroneMaster = LazyAddressable<GameObject>("RoR2/Base/Drones/EquipmentDroneMaster.prefab");
        private static readonly Lazy<GameObject> _colDronemanMaster = LazyAddressable<GameObject>("RoR2/DLC1/DroneCommander/DroneCommanderMaster.prefab");
        private static readonly Lazy<GameObject> _alphaConstructMaster = LazyAddressable<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructMaster.prefab");
        private static readonly Lazy<InteractableSpawnCard> _drone1SpawnCard = LazyAddressable<InteractableSpawnCard>("RoR2/Base/Drones/iscBrokenDrone1.asset");
        private static readonly Lazy<SkillDef> _drone1Skill = LazyAddressable<SkillDef>("RoR2/Base/Drones/Drone1BodyGun.asset");
        private static readonly Lazy<GameObject> _helperPrefab = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab"));
        private static readonly Lazy<Material> _summonDroneMaterial = new Lazy<Material>(() => LegacyResourcesAPI.Load<Material>("Materials/matSummonDrone"));

        internal static GameObject backupDroneMaster { get => _backupDroneMaster.Value; }
        internal static GameObject drone1Master { get => _drone1Master.Value; }
        internal static GameObject drone2Master { get => _drone2Master.Value; }
        internal static GameObject emergencyDroneMaster { get => _emergencyDroneMaster.Value; }
        internal static GameObject flameDroneMaster { get => _flameDroneMaster.Value; }
        internal static GameObject missileDroneMaster { get => _missileDroneMaster.Value; }
        internal static GameObject turret1Master { get => _turret1Master.Value; }
        internal static GameObject tc280DroneMaster { get => _tc280DroneMaster.Value; }
        internal static GameObject equipmentDroneMaster { get => _equipmentDroneMaster.Value; }
        internal static GameObject colDronemanMaster { get => _colDronemanMaster.Value; }
        internal static GameObject alphaConstructMaster { get => _alphaConstructMaster.Value; }
        internal static GameObject helperPrefab { get => _helperPrefab.Value; }
        internal static InteractableSpawnCard drone1SpawnCard { get => _drone1SpawnCard.Value; }
        internal static SkillDef drone1Skill { get => _drone1Skill.Value; }
        internal static Material summonDroneMaterial { get => _summonDroneMaterial.Value; }

#if DEBUG

        internal static SoundPlayer soundPlayer = new SoundPlayer();

        private void Update()
        {
            DropletGenerator.Update();
            if (Input.GetKeyDown(KeyCode.F9))
            {
                CharacterMaster master = PlayerCharacterMasterController.instances[0].master;
                Items.GradiusOption.Components.OptionMasterTracker.SpawnOption(master.GetBody().gameObject, 1);
                return;
            }
            soundPlayer.Update();
        }

#endif

        private void Awake()
        {
            Log = new Log(Logger);

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
            generalCfg.Bind(cfgFile);

            Log.Debug("Modifying vanilla drone behavior...");
            ModifyVanillaDroneDeathBehaviors();
            ModifyVanillaDronesSkillDrivers();

            Log.Debug("Instantiating item and artifact classes...");
            chensItemList = SetupModules(new ModuleBase[] { new GradiusOption(), new OptionSeed(), new Machines() });

            Log.Debug("Instantiating custom drones...");
            gradiusDronesList = Drones.DroneCatalog.Initialize(ModGuid, cfgFile);
            Drones.DroneCatalog.EfficientSetupAll(gradiusDronesList);

            Log.Debug("Applying vanilla changes and fixes...");
            RegisterVanillaChanges();
        }

        private List<ModuleBase> SetupModules(IEnumerable<ModuleBase> modules)
        {
            List<ModuleBase> enabledModules = new List<ModuleBase>();
            foreach (ModuleBase module in modules)
            {
                module.BindConfigFile(cfgFile);
                module.SetupConfig();
                if (module.enabled) enabledModules.Add(module);
                else Log.Warning($"{module.name} is disabled in the config. Skipping.");
            }
            foreach (ModuleBase module in enabledModules) module.SetupAttributes();
            foreach (ModuleBase module in enabledModules) module.SetupBehavior();
            foreach (ModuleBase module in enabledModules) module.Install();
            return enabledModules;
        }

        private static Lazy<T> LazyAddressable<T>(string key)
        {
            return new Lazy<T>(() =>
            {
                T asset = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
                if (asset == null) Log.Error($"Failed to load the addressable asset {key}.");
                return asset;
            });
        }

        private void ModifyVanillaDroneDeathBehaviors()
        {
            // Vanilla drones handle their broken interactables on their own since Alloyed Collective, including drone upgrades.
            // Only the TC-280 Prototype needs a custom death state, since it never leaves a broken drone in vanilla.
            if (generalCfg.megaDronesAreRepurchaseable) tc280DroneMaster.AssignDeathBehavior(typeof(Drones.TC280.DeathState));
        }

        private void ModifyVanillaDronesSkillDrivers()
        {
            drone1Master.SetAllDriversToAimTowardsEnemies();
            tc280DroneMaster.SetAllDriversToAimTowardsEnemies();
            missileDroneMaster.SetAllDriversToAimTowardsEnemies();
            backupDroneMaster.SetAllDriversToAimTowardsEnemies();
            flameDroneMaster.SetAllDriversToAimTowardsEnemies();
            equipmentDroneMaster.SetAllDriversToAimTowardsEnemies();
            colDronemanMaster.SetAllDriversToAimTowardsEnemies();
            alphaConstructMaster.SetAllDriversToAimTowardsEnemies();
        }

        internal static bool DebugCheck()
        {
            return false;
        }
    }
}
