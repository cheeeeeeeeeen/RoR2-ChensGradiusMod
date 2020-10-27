using Aetherium.Items;
using KomradeSpectre.Aetherium;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod
{
    public static class AetheriumCompatibility
    {
        public static bool hasSetup = false;

        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AetheriumPlugin.ModGuid);
                return (bool)_enabled;
            }
        }

        public static void Setup(GlobalConfig config)
        {
            if (!enabled) return;
            if (hasSetup)
            {
                Log.Warning("Aetherium.Setup: Already performed. Skipping.");
                return;
            }
            if (config.equipmentDroneInspire)
            {
                Log.Debug("Aetherium.Setup: Applying equipmentDroneInspire.");
                InspiringDrone.instance.AddCustomDrone("EquipmentDrone");
            }
            hasSetup = true;
        }
    }
}
