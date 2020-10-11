#undef DEBUG

using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Networking;
using R2API.Utils;
#if DEBUG
using RoR2;
using System.Collections.Generic;
#endif
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
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(ResourcesAPI))]
    public class GradiusModPlugin : BaseUnityPlugin
    {
        public const string ModVer =
#if DEBUG
            "0." +
#endif
            "1.3.4";

        public const string ModName = "ChensGradiusMod";
        public const string ModGuid = "com.Chen.ChensGradiusMod";

        private static ConfigFile cfgFile;

        internal static FilingDictionary<ItemBoilerplate> chensItemList = new FilingDictionary<ItemBoilerplate>();
        internal static BepInEx.Logging.ManualLogSource _logger;

#if DEBUG

        public void Update()
        {
            var i3 = Input.GetKeyDown(KeyCode.F3);
            var i4 = Input.GetKeyDown(KeyCode.F4);
            var i5 = Input.GetKeyDown(KeyCode.F5);
            var i6 = Input.GetKeyDown(KeyCode.F6);
            var i7 = Input.GetKeyDown(KeyCode.F7);
            if (i3 || i4 || i5 || i6 || i7)
            {
                Transform trans = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                List<PickupIndex> spawnList;
                if (i3) spawnList = Run.instance.availableTier1DropList;
                else if (i4) spawnList = Run.instance.availableTier2DropList;
                else if (i5) spawnList = Run.instance.availableTier3DropList;
                else if (i6) spawnList = Run.instance.availableEquipmentDropList;
                else spawnList = Run.instance.availableLunarDropList;
                PickupDropletController.CreatePickupDroplet(spawnList[Run.instance.spawnRng.RangeInt(0, spawnList.Count)], trans.position, new Vector3(0f, -5f, 0f));
            }
        }

#endif

        private void Awake()
        {
            _logger = Logger;

#if DEBUG
            Logger.LogWarning("Running test build with debug enabled!");
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };
#endif

            Logger.LogDebug("Loading assets...");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChensGradiusMod.chensgradiusmod_assets"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ChensGradiusMod", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            Logger.LogDebug("Loading global configs... No global configs found.");

            Logger.LogDebug("Instantiating item classes...");
            chensItemList = ItemBoilerplate.InitAll("ChensGradiusMod");

            Logger.LogDebug("Loading item configs and registering items and their behaviors...");
            foreach (ItemBoilerplate x in chensItemList)
            {
                x.SetupConfig(cfgFile);
                x.SetupAttributes("CHENSGRADIUSMOD", "CGM");
                if (x is Equipment equipment)
                    Logger.LogMessage($"Equipment CGM : {x.itemCodeName} : {(int)equipment.regIndex}");
                else if (x is Item item)
                    Logger.LogMessage($"Item CGM : {x.itemCodeName} : {(int)item.regIndex}");
                else if (x is Artifact artifact)
                    Logger.LogMessage($"Artifact CGM : {x.itemCodeName} : {(int)artifact.regIndex}");
                else
                    Logger.LogMessage($"Other CGM : {x.itemCodeName}");
                x.SetupBehavior();
            }

            Logger.LogDebug("Tweaking vanilla stuff... No tweaks found.");

            Logger.LogDebug("Setup done.");
        }
    }
}