using Aetherium;
using Aetherium.Items;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Compatibility
{
    /// <summary>
    /// A static class that contains compatibility features from Aetherium mod.
    /// </summary>
    public static class Aetherium
    {
        internal static bool hasSetup = false;

        private static bool? _enabled;

        /// <summary>
        /// The check to use if Aetherium mod is enabled.
        /// Always do an if check before invoking methods in this class or else unexpected errors will happen in-game.
        /// </summary>
        public static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AetheriumPlugin.ModGuid);
                return (bool)_enabled;
            }
        }

        internal static void Setup()
        {
            if (hasSetup)
            {
                Log.Warning("Aetherium.Setup: Already performed. Skipping.");
                return;
            }
            if (generalCfg.equipmentDroneInspire)
            {
                Log.Debug("Aetherium.Setup: Applying equipmentDroneInspire.");
                AddCustomDrone("EquipmentDrone");
            }
            hasSetup = true;
        }

        /// <summary>
        /// Allows the Inspiring Drone to inspire the custom drone.
        /// </summary>
        /// <param name="masterName">The Character Master name of the drone to add</param>
        public static void AddCustomDrone(string masterName)
        {
            InspiringDrone.instance.AddCustomDrone(masterName);
        }
    }
}