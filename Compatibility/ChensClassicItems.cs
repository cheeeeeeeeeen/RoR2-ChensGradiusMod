using Chen.ClassicItems;
using RoR2;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod
{
    /// <summary>
    /// A static class that contains compatibility features from Chen's Classic Items.
    /// </summary>
    public static class ChensClassicItemsCompatibility
    {
        internal static bool hasSetup = false;

        private static bool? _enabled;

        /// <summary>
        /// The check to use if Chen's Classic Items mod is enabled.
        /// Always do an if check before invoking methods in this class or else unexpected errors will happen in-game.
        /// </summary>
        public static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ClassicItemsPlugin.ModGuid);
                return (bool)_enabled;
            }
        }

        internal static void Setup()
        {
            if (hasSetup)
            {
                Log.Warning("ChensClassicItems.Setup: Already performed. Skipping.");
                return;
            }
            Log.Debug("ChensClassicItems.Setup: There is nothing to setup here.");
            hasSetup = true;
        }

        /// <summary>
        /// Uses the API to allow Arms Race item to be triggered when invoked.
        /// </summary>
        /// <param name="body">Body of the object that triggered the proc</param>
        /// <param name="damage">Final damage output</param>
        /// <param name="crit">Determines if the artillery will be a critical</param>
        /// <param name="procChainMask">Proc Chain Mask</param>
        public static void TriggerArtillery(CharacterBody body, float damage, bool crit, ProcChainMask procChainMask = default)
        {
            ArmsRace.instance.TriggerArtillery(body, damage, crit, procChainMask);
        }
    }
}