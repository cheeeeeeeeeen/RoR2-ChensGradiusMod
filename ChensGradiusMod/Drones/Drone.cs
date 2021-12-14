using BepInEx.Configuration;
using Chen.GradiusMod.Artifacts.Machines;
using Chen.GradiusMod.Items.GradiusOption;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones
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
        /// Constructor that creates the instance of the singleton class.
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
        public bool enabled { get; protected set; } = true;

        /// <summary>
        /// Determines if the drone can be spawned in the enemy drone spawn pool with Artifact of Machines.
        /// </summary>
        public int spawnWeightWithMachinesArtifact { get; protected set; } = 1;

        /// <summary>
        /// Allow the drone to be repurchased upon being decommissioned.
        /// The implementation of this config is manual and cannot be automated.
        /// Use the config option by inheriting DroneDeathState and implementing SpawnInteractable.
        /// </summary>
        public bool canBeRepurchased { get; protected set; } = true;

        /// <summary>
        /// Aetherium Compatibility: Determines if this drone can be inspired by the Inspiring Drone.
        /// </summary>
        public bool canBeInspired { get; protected set; } = true;

        /// <summary>
        /// Aetherium Compatibility: Determines if this drone can be revived and duplicated by Engineers Toolbelt.
        /// </summary>
        public bool affectedByEngineersToolbelt { get; protected set; } = true;

        /// <summary>
        /// Chen's Classic Items Compatibility: Determines if this drone can be healed by Drone Repair Kit.
        /// </summary>
        public bool affectedByDroneRepairKit { get; protected set; } = true;

        /// <summary>
        /// Determines if the drone can be spawned with Gradius' Options. Required to explicitly implement.
        /// </summary>
        public abstract bool canHaveOptions { get; }

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

        internal DroneInfo info;

        private string _name;
        private string _configCategory;

        /// <summary>
        /// The config file assigned to this custom drone. Use this to bind config options.
        /// </summary>
        protected ConfigFile config;

        /// <summary>
        /// This refers to the CharacterMaster GameObject of the drone.
        /// Implement this method in the drone class and have it return the CharacterMaster GameObject.
        /// </summary>
        protected abstract GameObject DroneCharacterMasterObject { get; }

        /// <summary>
        /// The first step in the setup process. Place here the logic needed before any processing begins.
        /// </summary>
        public virtual void PreSetup()
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

            canBeRepurchased = config.Bind(configCategory,
                "CanBeRepurchased", canBeRepurchased,
                "Allow this drone to be repurchased upon being decommissioned."
            ).Value;

            spawnWeightWithMachinesArtifact = config.Bind(configCategory,
                "AllowToBeSpawnedWithMachinesArtifact", spawnWeightWithMachinesArtifact,
                "Spawn weight of how likely enemies will get this drone. 0 means it will not spawn at all. 1 is the default."
            ).Value;

            canBeInspired = config.Bind(configCategory,
                "CanBeInspired", canBeInspired,
                "Aetherium Compatibility: Allow this drone to be Inspired by Inspiring Drone."
            ).Value;

            affectedByEngineersToolbelt = config.Bind(configCategory,
                "AffectedByEngineersToolbelt", affectedByEngineersToolbelt,
                "Chen's Classic Items Compatibility: Allow this drone to be affected by Engineers Toolbelt. " +
                "Some drones may override this config, such as the Psy Drones who will never be affected."
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
            if (canHaveOptions) GradiusOption.instance.SupportMinionType(name);
            Machines.instance.AddEnemyDroneType(DroneCharacterMasterObject, spawnWeightWithMachinesArtifact);
            if (Compatibility.Aetherium.enabled)
            {
                if (canBeInspired) Compatibility.Aetherium.AddInspiredCustomDrone(name);
                if (affectedByEngineersToolbelt) Compatibility.Aetherium.AddEngineersToolbeltCustomDrone(name);
            }
            if (Compatibility.ChensClassicItems.enabled && affectedByDroneRepairKit)
            {
                Compatibility.ChensClassicItems.DroneRepairKitSupport(name);
            }
        }

        /// <summary>
        /// The fifth step in the setup process. Place here the code for cleanup, or for finalization.
        /// This will still be performed whether the drone is enabled or disabled.
        /// This will still also be performed if the drone was already set up or not.
        /// </summary>
        public virtual void PostSetup()
        {
        }

        internal void BindConfig(ConfigFile config)
        {
            this.config = config;
        }

        /// <summary>
        /// First phase of the setup process along with required logic. This phase invokes SetupConfig.
        /// This method is exposed for usage outside of this class.
        /// </summary>
        /// <returns>True means the setup was performed. False means the setup was skipped or the drone is disabled.</returns>
        public bool SetupFirstPhase()
        {
            if (alreadySetup)
            {
                Log.Warning($"1st Phase: {info.mod}-{name} was already set up. Skipping.");
                return false;
            }
            SetupConfig();
            if (!enabled)
            {
                Log.Warning($"1st Phase: {info.mod}-{name} is disabled. Skipping.");
                alreadySetup = true;
            }
            return enabled;
        }

        /// <summary>
        /// Second phase of the setup process along with required logic. This method invokes SetupComponents.
        /// This method is exposed for usage outside of this class.
        /// </summary>
        /// <returns>True means the setup was performed. False means the setup was skipped or the drone is disabled.</returns>
        public bool SetupSecondPhase()
        {
            if (!enabled) Log.Warning($"2nd Phase: {info.mod}-{name} is disabled.");
            if (alreadySetup) Log.Warning($"2nd Phase: {info.mod}-{name} was already set up.");
            if (!enabled || alreadySetup)
            {
                Log.Warning($"2nd Phase: {info.mod}-{name} setup skipped.");
                return false;
            }
            SetupComponents();
            return true;
        }

        /// <summary>
        /// Third phase of the setup process along with required logic. This method invokes SetupBehavior.
        /// This method is exposed for usage outside of this class.
        /// </summary>
        /// <returns>True means the setup was performed. False means the setup was skipped or the drone is disabled.</returns>
        public bool SetupThirdPhase()
        {
            if (!enabled) Log.Warning($"3rd Phase: {info.mod}-{name} is disabled.");
            if (alreadySetup) Log.Warning($"3rd Phase: {info.mod}-{name} was already set up.");
            if (!enabled || alreadySetup)
            {
                Log.Warning($"3rd Phase: {info.mod}-{name} setup skipped.");
                return false;
            }
            SetupBehavior();
            return alreadySetup = true;
        }
    }
}