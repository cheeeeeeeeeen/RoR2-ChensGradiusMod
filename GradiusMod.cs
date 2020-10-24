#undef DEBUG

using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Networking;
using R2API.Utils;
using System.Reflection;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;
using Path = System.IO.Path;

namespace Chen.GradiusMod
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(ResourcesAPI), nameof(SoundAPI))]
    public class GradiusModPlugin : BaseUnityPlugin
    {
        public const string ModVer =
#if DEBUG
            "0." +
#endif
            "1.6.4";

        public const string ModName = "ChensGradiusMod";
        public const string ModGuid = "com.Chen.ChensGradiusMod";

        private static ConfigFile cfgFile;

        internal static FilingDictionary<CatalogBoilerplate> chensItemList = new FilingDictionary<CatalogBoilerplate>();
        internal static BepInEx.Logging.ManualLogSource _logger;

#if DEBUG

        public void Update()
        {
            var i3 = Input.GetKeyDown(KeyCode.F3);
            var i4 = Input.GetKeyDown(KeyCode.F4);
            var i5 = Input.GetKeyDown(KeyCode.F5);
            var i6 = Input.GetKeyDown(KeyCode.F6);
            var i7 = Input.GetKeyDown(KeyCode.F7);
            var i8 = Input.GetKeyDown(KeyCode.F8);
            var i9 = Input.GetKeyDown(KeyCode.F9);
            if (i3 || i4 || i5 || i6 || i7 || i8 || i9)
            {
                RoR2.CharacterMaster master = RoR2.PlayerCharacterMasterController.instances[0].master;
                RoR2.CharacterBody body = master.GetBody();
                Transform trans = body.gameObject.transform;
                System.Collections.Generic.List<RoR2.PickupIndex> spawnList;
                if (i9)
                {
                    OptionMasterTracker.SpawnOption(trans.gameObject, 1);
                    return;
                }
                if (i3) spawnList = RoR2.Run.instance.availableTier1DropList;
                else if (i4) spawnList = RoR2.Run.instance.availableTier2DropList;
                else if (i5) spawnList = RoR2.Run.instance.availableTier3DropList;
                else if (i6) spawnList = RoR2.Run.instance.availableEquipmentDropList;
                else if (i7) spawnList = RoR2.Run.instance.availableLunarDropList;
                else spawnList = RoR2.Run.instance.availableBossDropList;
                RoR2.PickupDropletController.CreatePickupDroplet(spawnList[RoR2.Run.instance.spawnRng.RangeInt(0, spawnList.Count)], trans.position, new Vector3(0f, -5f, 0f));
            }
        }

#endif

        private void Awake()
        {
            _logger = Logger;

#if DEBUG
            Log.Warning("Running test build with debug enabled!");
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };
#endif

            Log.Debug("Loading assets...");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChensGradiusMod.chensgradiusmod_assets"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ChensGradiusMod", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            Log.Debug("Loading sounds...");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChensGradiusMod.chensgradiusmod_soundbank.bnk"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            Log.Debug("Loading global configs... No global configs found.");

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
            CatalogBoilerplate.ConsoleDump(Logger, chensItemList);
        }
    }

    public static class Log
    {
        public static void Debug(object data) => logger.LogDebug(data);

        public static void Error(object data) => logger.LogError(data);

        public static void Info(object data) => logger.LogInfo(data);

        public static void Message(object data) => logger.LogMessage(data);

        public static void Warning(object data) => logger.LogWarning(data);

        public static BepInEx.Logging.ManualLogSource logger => GradiusModPlugin._logger;
    }
}