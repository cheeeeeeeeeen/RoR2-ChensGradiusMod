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
        private ConfigEntry<bool> enabledConfig;
        private ConfigEntry<bool> canBeInspiredConfig;
        public bool enabled { get; private set; } = true;
        public bool canBeInspired { get; private set; } = true;

        public string name
        {
            get
            {
                if (_name == null) _name = GetType().Name;
                return _name;
            }
        }

        public bool alreadySetup { get; private set; } = false;

        protected string configCategory
        {
            get
            {
                if (_configCategory == null) _configCategory = $"Drones.{name}";
                return _configCategory;
            }
        }

        private string _name;
        private string _configCategory;

        protected ConfigFile config;

        protected virtual void PreSetup()
        {
        }

        protected virtual void SetupConfig()
        {
            enabledConfig = config.Bind(
                configCategory,
                "Enabled", enabled,
                "Set to false to disable this feature."
            );
            enabled = enabledConfig.Value;

            canBeInspiredConfig = config.Bind(
                configCategory,
                "CanBeInspired", canBeInspired,
                "Aetherium Compatibility: Allow this drone to be Inspired by Inspiring Drone."
            );
            canBeInspired = canBeInspiredConfig.Value;
        }

        protected virtual void SetupComponents()
        {
        }

        protected virtual void SetupBehavior()
        {
        }

        protected virtual void PostSetup()
        {
            alreadySetup = true;
        }

        internal void BindConfig(ConfigFile config)
        {
            this.config = config;
        }

        public bool SetupFirstPhase()
        {
            PreSetup();
            SetupConfig();
            if (!enabled) alreadySetup = true;
            return enabled;
        }

        public void SetupSecondPhase()
        {
            SetupComponents();
            SetupBehavior();
            PostSetup();
        }
    }
}