using Chen.GradiusMod.Artifacts.Machines;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using VanillaDeathState = EntityStates.Drone.DeathState;

namespace Chen.GradiusMod.Drones
{
    /// <summary>
    /// An Entity State that inherits from the original EntityStates.Drone.DeathState.
    /// It spawns the drone's broken interactable upon death the same way vanilla drones do, including drone upgrades from the Drone Combiner,
    /// while letting custom drones decide which spawn card to use and if the interactable should spawn at all.
    /// Do not use this class directly. Always inherit from this class and implement the interactable property.
    /// </summary>
    public abstract class DroneDeathState : VanillaDeathState
    {
        /// <summary>
        /// The additional cost multiplier of the broken drone per drone upgrade. Matches vanilla.
        /// </summary>
        public const float CostMultiplierPerUpgrade = .2f;

        /// <summary>
        /// Flag that is used to check if the interactable will be spawned upon death. Modify this in OnEnter.
        /// Required implementation for its default value.
        /// </summary>
        protected abstract bool SpawnInteractable { get; set; }

        /// <summary>
        /// A method that should be implemented by the child class. This will be the Spawn Card that will be used to spawn when the drone is destroyed.
        /// </summary>
        /// <returns>The spawn card of the interactable if implemented.</returns>
        protected abstract InteractableSpawnCard GetInteractableSpawnCard { get; }

        /// <summary>
        /// The number of drone upgrades the drone had when it died.
        /// </summary>
        protected int upgradeCount { get; private set; }

        private bool interactableSpawned;

        /// <summary>
        /// A method that can be overridden to add or change the logic when the interactable is spawned.
        /// Default logic is to compute for the scaled cost of the drone, and to carry over the drone upgrades to the interactable.
        /// </summary>
        protected virtual void OnInteractableSpawn(GameObject spawnedObject)
        {
            PurchaseInteraction purchaseInteraction = spawnedObject.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                float upgradeMultiplier = 1f + upgradeCount * CostMultiplierPerUpgrade;
                purchaseInteraction.Networkcost = Mathf.RoundToInt(Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost) * costCoefficient * upgradeMultiplier);
            }
            SummonMasterBehavior summonMasterBehavior = spawnedObject.GetComponent<SummonMasterBehavior>();
            if (summonMasterBehavior) summonMasterBehavior.NetworkdroneUpgradeCount = upgradeCount;
        }

        /// <summary>
        /// Overridden method from the original state so that it would instead spawn the specified interactable's spawn card.
        /// There is no need to override this unless special behavior is needed.
        /// </summary>
        /// <param name="contactPoint">The point where the interactable spawns</param>
        public override void OnImpactServer(Vector3 contactPoint)
        {
            if (!NetworkServer.active || interactableSpawned || !SpawnInteractable) return;
            InteractableSpawnCard spawnCard = GetInteractableSpawnCard;
            if (!spawnCard || !DirectorCore.instance) return;
            DirectorPlacementRule placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = contactPoint
            };
            GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
            if (gameObject)
            {
                interactableSpawned = true;
                OnInteractableSpawn(gameObject);
            }
        }

        /// <summary>
        /// Overrideable OnEnter method from the original state. Always call base.OnEnter. Initialize values at runtime here.
        /// To perform the death behavior specified in OnImpactServer, destroyOnImpact must be set to true.
        /// This method already does that so long as base.OnEnter is invoked.
        /// </summary>
        public override void OnEnter()
        {
            CopyVanillaDeathEffects();
            hardCutoffDuration = maxFallDuration = bodyPreservationDuration = deathDuration = 12f;
            destroyOnImpact = true;
            if (characterBody && characterBody.inventory)
            {
                upgradeCount = characterBody.inventory.GetItemCountEffective(DLC3Content.Items.DroneUpgradeHidden);
                // Ghosts and characters about to be revived do not leave a broken drone behind, same as vanilla.
                if (Util.HasAnyItemWithTag(characterBody.inventory, ItemTag.Ghost)) SpawnInteractable = false;
                if (characterBody.master && characterBody.master.IsExtraLifePendingServer()) SpawnInteractable = false;
            }
            if (Machines.instance != null && Machines.instance.ShouldSuppressBrokenDrone(characterBody)) SpawnInteractable = false;
            base.OnEnter();
        }

        /// <summary>
        /// Overrideable OnExit method from the original state. Always call base.OnExit.
        /// </summary>
        public override void OnExit() => base.OnExit();

        /// <summary>
        /// Overrideable FixedUpdate method from the original state. Always call base.FixedUpdate.
        /// </summary>
        public override void FixedUpdate() => base.FixedUpdate();

        private void CopyVanillaDeathEffects()
        {
            // Custom death states do not have an EntityStateConfiguration, so the vanilla values are copied from a configured vanilla instance.
            if (!(EntityStateCatalog.InstantiateState(typeof(VanillaDeathState)) is VanillaDeathState vanillaState)) return;
            initialExplosionEffect = vanillaState.initialExplosionEffect;
            deathExplosionEffect = vanillaState.deathExplosionEffect;
            initialSoundString = vanillaState.initialSoundString;
            deathSoundString = vanillaState.deathSoundString;
            deathEffectRadius = vanillaState.deathEffectRadius;
            forceAmount = vanillaState.forceAmount;
            costCoefficient = vanillaState.costCoefficient;
        }
    }
}
