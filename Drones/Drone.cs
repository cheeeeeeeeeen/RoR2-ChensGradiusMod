using BepInEx.Configuration;
using System;

namespace Chen.GradiusMod
{
    /// <summary>
    /// Allows for making drone classes into singleton classes.
    /// </summary>
    /// <typeparam name="T">The drone class name</typeparam>
    public abstract class Drone<T> : Drone where T : Drone<T>
    {
        /// <summary>
        /// The instance of the singleton class.
        /// </summary>
        public static T instance { get; private set; }

        /// <summary>
        /// Constructor that should be able to render this class as a singleton.
        /// </summary>
        public Drone()
        {
            if (instance != null) throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" instantiated twice.");
            instance = this as T;
        }
    }

    /// <summary>
    /// The drone class where mod creators should inherit from to ease up development.
    /// </summary>
    public abstract class Drone
    {
        /// <summary>
        /// Determines if the drone should be enabled/disabled. Disabled drones will not be set up.
        /// </summary>
        public bool enabled { get; private set; } = true;

        /// <summary>
        /// Aetherium Compatibility: Determines if this drone can be inspired by the Inspiring Drone.
        /// </summary>
        public bool canBeInspired { get; private set; } = true;

        /// <summary>
        /// Chen's Classic Items Compatibility: Determines if this drone can be healed by Drone Repair Kit.
        /// </summary>
        public bool affectedByDroneRepairKit { get; private set; } = true;

        /// <summary>
        /// Fetches the custom drone's class name.
        /// </summary>
        public string name
        {
            get
            {
                if (_name == null) _name = GetType().Name;
                return _name;
            }
        }

        /// <summary>
        /// Used to determine if the custom drone was already set up.
        /// </summary>
        public bool alreadySetup { get; private set; } = false;

        /// <summary>
        /// The category that will be used in the config file that contains the custom drone's config options.
        /// </summary>
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

        /// <summary>
        /// The config file assigned to this custom drone. Use this to bind config options.
        /// </summary>
        protected ConfigFile config;

        /// <summary>
        /// The first step in the setup process. Place here the logic needed before any processing begins.
        /// </summary>
        protected virtual void PreSetup()
        {
        }

        /// <summary>
        /// The second step in the setup process. Place here all the code related to adding configurations for the custom drone.
        /// </summary>
        protected virtual void SetupConfig()
        {
            enabled = config.Bind(configCategory,
                "Enabled", enabled,
                "Set to false to disable this feature."
            ).Value;

            canBeInspired = config.Bind(configCategory,
                "CanBeInspired", canBeInspired,
                "Aetherium Compatibility: Allow this drone to be Inspired by Inspiring Drone."
            ).Value;

            affectedByDroneRepairKit = config.Bind(configCategory,
                "AffectedByDroneRepairKit", affectedByDroneRepairKit,
                "Chen's Classic Items Compatibility: Allow this drone to be healed by Drone Repair Kit."
            ).Value;
        }

        /// <summary>
        /// The third step in the setup process. Place here all initialization of components, assets, textures, sounds, etc.
        /// </summary>
        protected virtual void SetupComponents()
        {
        }

        /// <summary>
        /// The fourth step in the setup process. Place here the code related to the drone's behavior.
        /// One may place here mod compatibility code. Hooks should also go here.
        /// </summary>
        protected virtual void SetupBehavior()
        {
            if (canBeInspired && AetheriumCompatibility.enabled)
            {
                AetheriumCompatibility.AddCustomDrone(name);
            }
            if (affectedByDroneRepairKit && ChensClassicItemsCompatibility.enabled)
            {
                ChensClassicItemsCompatibility.DroneRepairKitSupport(name);
            }
        }

        /// <summary>
        /// The fifth step in the setup process. Place here the code for cleanup, or for finalization.
        /// </summary>
        protected virtual void PostSetup()
        {
            alreadySetup = true;
        }

        internal void BindConfig(ConfigFile config)
        {
            this.config = config;
        }

        /// <summary>
        /// Shorthand for the first and second steps of the setup process along with generic logic. This method is exposed for usage outside of this class.
        /// </summary>
        /// <returns>Boolean value if the drone is enabled or not.</returns>
        public bool SetupFirstPhase()
        {
            PreSetup();
            SetupConfig();
            if (!enabled) alreadySetup = true;
            return enabled;
        }

        /// <summary>
        /// Shorthand for the third, fourth and fifth steps of the setup process. This method is exposed for usage outside of this class.
        /// </summary>
        public void SetupSecondPhase()
        {
            SetupComponents();
            SetupBehavior();
            PostSetup();
        }
    }
}