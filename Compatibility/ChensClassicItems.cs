using Chen.ClassicItems;
using RoR2;

namespace Chen.GradiusMod
{
    public static class ChensClassicItemsCompatibility
    {
        public static bool hasSetup = false;

        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ClassicItemsPlugin.ModGuid);
                return (bool)_enabled;
            }
        }

        public static void Setup()
        {
            if (hasSetup)
            {
                Log.Warning("ChensClassicItems.Setup: Already performed. Skipping.");
                return;
            }
            Log.Debug("ChensClassicItems.Setup: There is nothing to setup here.");
            hasSetup = true;
        }

        public static void TriggerArtillery(CharacterBody body, float damage, bool crit, ProcChainMask procChainMask = default)
        {
            ArmsRace.instance.TriggerArtillery(body, damage, crit, procChainMask);
        }
    }
}