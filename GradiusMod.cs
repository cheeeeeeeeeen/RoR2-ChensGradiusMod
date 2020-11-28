#undef DEBUG

using BepInEx;
using BepInEx.Configuration;
using Chen.ClassicItems;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.LogHelpers;
using KomradeSpectre.Aetherium;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using TILER2;
using UnityEngine;
using static Chen.Helpers.GeneralHelpers.AssetsManager;
using static TILER2.MiscUtil;
using Path = System.IO.Path;

namespace Chen.GradiusMod
{
    /// <summary>
    /// The mother plugin that allows the mod to be loaded to RoR2.
    /// </summary>
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
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
            "2.1.4";

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

        public void Update()
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
        }

        public void FixedUpdate()
        {
            var i1 = Input.GetKeyDown(KeyCode.Alpha1);
            var i2 = Input.GetKeyDown(KeyCode.Alpha2);
            var i3 = Input.GetKeyDown(KeyCode.Alpha3);
            var i4 = Input.GetKeyDown(KeyCode.Alpha4);
            if (i1 || i2 || i3 || i4)
            {
                RoR2.CharacterMaster master = RoR2.PlayerCharacterMasterController.instances[0].master;
                RoR2.CharacterBody body = master.GetBody();
                if (i1) AkSoundEngine.PostEvent(FireLaser.chargeLaserEventId, body.gameObject);
                else if (i2) AkSoundEngine.PostEvent(FireLaser.dissipateLaserEventId, body.gameObject);
                else if (i3) AkSoundEngine.PostEvent(GradiusOption.getOptionEventId, body.gameObject);
                else if (i4) AkSoundEngine.PostEvent(GradiusOption.loseOptionEventId, body.gameObject);
            }
        }

#endif

        private void Awake()
        {
            Log = new Log(Logger);

#if DEBUG
            MultiplayerTest.Enable(Log);
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
            DroneCatalog.SetupAll(gradiusDronesList);

            Log.Debug("Applying vanilla fixes...");
            RegisterVanillaFixes();

            Log.Debug("Applying compatibility changes...");
            if (AetheriumCompatibility.enabled) AetheriumCompatibility.Setup();
            if (ChensClassicItemsCompatibility.enabled) ChensClassicItemsCompatibility.Setup();
        }

        private void RegisterVanillaFixes()
        {
            if (generalCfg.emergencyDroneFix)
            {
                Log.Debug("Vanilla Fix: Applying emergencyDroneFix.");
                On.RoR2.HealBeamController.HealBeamAlreadyExists_GameObject_HealthComponent += HealBeamController_HealBeamAlreadyExists_GO_HC;
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

        internal class GlobalConfig : AutoConfigContainer
        {
            [AutoConfig("Applies a fix for Emergency Drones. Set to false if there are issues regarding compatibility.", AutoConfigFlags.PreventNetMismatch)]
            public bool emergencyDroneFix { get; private set; } = true;

            [AutoConfig("Aetherium Compatibility: Allow Equipment Drones to be Inspired by Inspiring Drone.", AutoConfigFlags.PreventNetMismatch)]
            public bool equipmentDroneInspire { get; private set; } = true;
        }
    }
}