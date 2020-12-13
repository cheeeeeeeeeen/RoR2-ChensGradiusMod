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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TILER2;
using static Chen.Helpers.GeneralHelpers.AssetsManager;
using static TILER2.MiscUtil;
using Path = System.IO.Path;

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
            "2.2.10";

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
            BundleInfo models = new BundleInfo("@ChensGradiusMod", "Chen.GradiusMod.chensgradiusmod_assets", BundleType.UnityAssetBundle);
            BundleInfo sounds = new BundleInfo("@ChensGradiusMod", "Chen.GradiusMod.chensgradiusmod_soundbank.bnk", BundleType.WWiseSoundBank);
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