using BepInEx.Configuration;
using System;

namespace Chen.GradiusMod
{
    public abstract class Drone<T> : Drone where T : Drone<T>
    {
        public static T instance { get; private set; }

        public Drone()
        {
            if (instance != null) throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" instantiated twice.");
            instance = this as T;
        }
    }

    public abstract class Drone
    {
        public bool enabled
        {
            get
            {
                if (enabledConfig == null) return false;
                if (_enabled == null) _enabled = enabledConfig.Value;
                return (bool)_enabled;
            }
        }

        protected string configCategory => $"Drones.{GetType().Name}";
        protected ConfigFile config => GradiusModPlugin.cfgFile;

        private ConfigEntry<bool> enabledConfig { get; set; }

        private bool? _enabled;

        public virtual void SetupConfig()
        {
            enabledConfig = config.Bind(
                configCategory,
                "Enabled", true,
                "Set to false to disable this feature."
            );
        }

        public virtual void SetupComponents()
        {
            if (!enabled) return;
        }

        public virtual void SetupBehavior()
        {
            if (!enabled) return;
        }

        public void SetupAll()
        {
            SetupConfig();
            SetupComponents();
            SetupBehavior();
        }
    }
}