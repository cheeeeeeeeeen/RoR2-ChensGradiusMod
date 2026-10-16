using BepInEx.Configuration;
using Chen.GradiusMod.Artifacts.Machines;
using Chen.GradiusMod.Items.GradiusOption;
using Chen.Helpers.GeneralHelpers;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Chen.GradiusMod.GradiusModPlugin;
using Object = UnityEngine.Object;

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

        /// <summary>
        /// The DroneDef of the custom drone. This registers the drone to the game's drone systems introduced in Alloyed Collective,
        /// such as drone pickups, the Drone Scrapper, the Drone Combiner, drone upgrades and the drone purchase notifications.
        /// Create it with CreateDroneDef during SetupComponents.
        /// </summary>
        public DroneDef droneDef { get; protected set; }

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
            if (canHaveOptions && GradiusOption.instance != null) GradiusOption.instance.SupportMinionType(name);
            if (Machines.instance != null) Machines.instance.AddEnemyDroneType(DroneCharacterMasterObject, spawnWeightWithMachinesArtifact);
        }

        /// <summary>
        /// Loads a vanilla DroneDef to be used as a template in CreateDroneDef.
        /// </summary>
        /// <param name="addressableKey">The addressable key of the DroneDef, e.g. "RoR2/Base/Drones/Drone1.asset".</param>
        /// <returns>The vanilla DroneDef</returns>
        protected static DroneDef LoadVanillaDroneDef(string addressableKey)
        {
            return Addressables.LoadAssetAsync<DroneDef>(addressableKey).WaitForCompletion();
        }

        /// <summary>
        /// Creates and registers the DroneDef of the custom drone, and links it to the broken interactable so it behaves like a vanilla drone.
        /// The language tokens used are {tokenPrefix}_NAME, {tokenPrefix}_PICKUP, {tokenPrefix}_DESCRIPTION and {tokenPrefix}_SKILL_DESCRIPTION.
        /// </summary>
        /// <param name="tierTemplate">A vanilla DroneDef where the tier, type and other gameplay flags are copied from.</param>
        /// <param name="bodyPrefab">The CharacterBody prefab of the drone.</param>
        /// <param name="brokenObject">The broken interactable prefab of the drone.</param>
        /// <param name="spawnCard">The spawn card of the broken interactable.</param>
        /// <param name="tokenPrefix">The prefix of the language tokens of the drone.</param>
        /// <param name="icon">The icon of the drone.</param>
        /// <param name="displayPrefab">The model displayed when the drone is a pickup. Use CreatePickupDisplay to make one.</param>
        /// <param name="commandTemplate">The vanilla DroneDef the drone body was cloned from. Its Drone Tech command text is reused since the command behavior is inherited.</param>
        /// <returns>The registered DroneDef</returns>
        protected DroneDef CreateDroneDef(DroneDef tierTemplate, GameObject bodyPrefab, GameObject brokenObject, InteractableSpawnCard spawnCard,
                                          string tokenPrefix, Texture icon, GameObject displayPrefab, DroneDef commandTemplate = null)
        {
            DroneDef newDroneDef = tierTemplate ? Object.Instantiate(tierTemplate) : ScriptableObject.CreateInstance<DroneDef>();
            newDroneDef.name = name;
            newDroneDef.bodyPrefab = bodyPrefab;
            newDroneDef.displayPrefab = displayPrefab;
            newDroneDef.droneBrokenSpawnCard = spawnCard;
            newDroneDef.remoteOpBody = null;
            newDroneDef.remoteOpCost = 0;
            newDroneDef.requiredExpansion = null;
#pragma warning disable CS0618 // The direct sprite reference is still the supported way for content that is not in the addressables catalog.
            newDroneDef.iconSprite = CreateSprite(icon);
#pragma warning restore CS0618
            // The template points to the icon of the vanilla drone, which would take priority over the direct sprite.
            newDroneDef.iconSpriteReference = null;
            newDroneDef.nameToken = $"{tokenPrefix}_NAME";
            newDroneDef.pickupToken = $"{tokenPrefix}_PICKUP";
            newDroneDef.descriptionToken = $"{tokenPrefix}_DESCRIPTION";
            newDroneDef.skillDescriptionToken = $"{tokenPrefix}_SKILL_DESCRIPTION";
            if (commandTemplate) newDroneDef.droneTechCommandToken = commandTemplate.droneTechCommandToken;
            if (tierTemplate) newDroneDef.desiredSortPosition = tierTemplate.desiredSortPosition + .5f;
            if (!ContentAddition.AddDroneDef(newDroneDef)) Log.Warning($"Drone.CreateDroneDef: Failed to add the DroneDef of {name}.");
            LinkBrokenObjectToDroneDef(brokenObject, newDroneDef);
            return newDroneDef;
        }

        /// <summary>
        /// Creates a model that is displayed when the drone is dropped as a pickup.
        /// Call this before the original model is used for the body, as replacing the body model changes the original object.
        /// </summary>
        /// <param name="model">The model of the drone.</param>
        /// <param name="scale">The scale of the display model.</param>
        /// <returns>The display prefab</returns>
        protected GameObject CreatePickupDisplay(GameObject model, float scale = 1f)
        {
            GameObject display = PrefabAPI.InstantiateClone(model, $"Pickup{name}", false);
            foreach (Collider collider in display.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(collider);
            display.transform.localScale *= scale;
            return display;
        }

        /// <summary>
        /// Replaces the model of the broken interactable that was cloned from a vanilla broken drone, and makes the new model interactable and highlightable.
        /// </summary>
        /// <param name="brokenObject">The cloned broken interactable.</param>
        /// <param name="customBrokenModel">The new model of the broken interactable.</param>
        /// <param name="highlightRenderer">The renderer that is highlighted when looking at the interactable.</param>
        /// <param name="extraInteractableParts">Other parts of the model with colliders that should also be interactable.</param>
        protected void ReplaceBrokenModel(GameObject brokenObject, GameObject customBrokenModel, Renderer highlightRenderer, params GameObject[] extraInteractableParts)
        {
            if (!brokenObject.GetComponent<ModelLocator>()) brokenObject.AddComponent<ModelLocator>();
            Transform oldVfx = FindChildRecursive(brokenObject.transform, "BrokenDroneVFX");
            brokenObject.ReplaceModel(customBrokenModel, false);
            Highlight highlight = brokenObject.GetComponent<Highlight>();
            if (!highlight)
            {
                highlight = brokenObject.AddComponent<Highlight>();
                highlight.highlightColor = Highlight.HighlightColor.interactive;
                highlight.strength = 1f;
            }
            highlight.targetRenderer = highlightRenderer;
            customBrokenModel.AddComponent<EntityLocator>().entity = brokenObject;
            foreach (GameObject part in extraInteractableParts)
            {
                if (part && !part.GetComponent<EntityLocator>()) part.AddComponent<EntityLocator>().entity = brokenObject;
            }
            if (oldVfx)
            {
                oldVfx.SetParent(customBrokenModel.transform, false);
                Transform sparks = FindChildRecursive(oldVfx, "Small Sparks, Mesh");
                ParticleSystem sparksSystem = sparks ? sparks.GetComponent<ParticleSystem>() : null;
                if (sparksSystem && highlightRenderer is MeshRenderer meshRenderer)
                {
                    ParticleSystem.ShapeModule sparksShape = sparksSystem.shape;
                    sparksShape.shapeType = ParticleSystemShapeType.MeshRenderer;
                    sparksShape.meshShapeType = ParticleSystemMeshShapeType.Edge;
                    sparksShape.meshRenderer = meshRenderer;
                }
            }
            else Log.Warning($"Drone.ReplaceBrokenModel: The broken drone effects of {name} were not found. The broken drone will not have sparks.");
        }

        /// <summary>
        /// Finds the damage point effect in the broken drone effects carried over by ReplaceBrokenModel.
        /// </summary>
        /// <param name="customBrokenModel">The model passed to ReplaceBrokenModel.</param>
        /// <returns>The transform of the damage point effect, or null if it was not found.</returns>
        protected static Transform FindBrokenDamagePoint(GameObject customBrokenModel)
        {
            return FindChildRecursive(customBrokenModel.transform, "Damage Point");
        }

        /// <summary>
        /// Finds a child transform by name anywhere in the hierarchy.
        /// </summary>
        /// <param name="root">The transform to search under.</param>
        /// <param name="childName">The name of the child.</param>
        /// <returns>The first transform found with the name, or null.</returns>
        protected static Transform FindChildRecursive(Transform root, string childName)
        {
            if (!root) return null;
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName) return child;
            }
            return null;
        }

        private void LinkBrokenObjectToDroneDef(GameObject brokenObject, DroneDef linkedDroneDef)
        {
            if (!brokenObject) return;
            // The broken interactable is cloned from a vanilla drone, so it still refers to the vanilla DroneDef.
            // Without this, disabling the vanilla drone in the lobby would also remove this drone from the stages.
            DroneAvailability availability = brokenObject.GetComponent<DroneAvailability>();
            if (availability) availability.droneDef = linkedDroneDef;

            GenericInspectInfoProvider inspectProvider = brokenObject.GetComponent<GenericInspectInfoProvider>();
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            InspectDef templateInspectDef = null;
            if (inspectProvider && inspectProvider.InspectInfo) templateInspectDef = inspectProvider.InspectInfo;
            else if (summonMasterBehavior && summonMasterBehavior.inspectDef) templateInspectDef = summonMasterBehavior.inspectDef;
            if (!templateInspectDef) return;
            InspectDef newInspectDef = Object.Instantiate(templateInspectDef);
            newInspectDef.name = $"{name}BrokenInspectDef";
            if (newInspectDef.Info != null)
            {
                newInspectDef.Info.Visual = linkedDroneDef.iconSprite;
                newInspectDef.Info.VisualReference = null;
                newInspectDef.Info.TitleToken = linkedDroneDef.nameToken;
                newInspectDef.Info.DescriptionToken = linkedDroneDef.descriptionToken;
                RoR2Application.onLoad += () => newInspectDef.Info.TitleColor = ColorCatalog.GetColor(linkedDroneDef.color);
            }
            if (inspectProvider) inspectProvider.InspectInfo = newInspectDef;
            if (summonMasterBehavior) summonMasterBehavior.inspectDef = newInspectDef;
        }

        private static Sprite CreateSprite(Texture icon)
        {
            if (!(icon is Texture2D texture)) return null;
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f));
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