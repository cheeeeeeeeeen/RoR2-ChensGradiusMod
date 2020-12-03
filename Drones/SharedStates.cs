using RoR2;
using System;
using UnityEngine;
using VanillaDeathState = EntityStates.Drone.DeathState;

namespace Chen.GradiusMod
{
    /// <summary>
    /// An Entity State that should inherit from the original EntityStates.Drone.DeathState.
    /// The original code does not support custom spawn cards to be detected when dying so that the interactable can spawn again.
    /// This state will cater to custom drones so they are also able to spawn interactables upon death.
    /// Do not use this class directly. Always inherit from this class and implement the interactable property.
    /// </summary>
    public class DroneDeathState : VanillaDeathState
    {
        /// <summary>
        /// A method that should be implemented by the child class. This will be the Spawn Card that will be used to spawn when the drone is destroyed.
        /// </summary>
        /// <returns>The spawn card of the interactable if implemented.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Property is not implemented.</exception>
        protected virtual InteractableSpawnCard GetInteractableSpawnCard()
        {
            throw new InvalidOperationException($"{GetType().Name}.interactable Property is not implemented/overridden.");
        }

        /// <summary>
        /// Overridden method from the original state so that it would instead spawn the specified interactable's spawn card.
        /// There is no need to override this unless special behavior is needed.
        /// </summary>
        /// <param name="contactPoint">The point where the interactable spawns</param>
        public override void OnImpactServer(Vector3 contactPoint)
        {
            var spawnCard = GetInteractableSpawnCard();
            if (spawnCard != null)
            {
                DirectorPlacementRule placementRule = new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    position = contactPoint
                };
                GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
                if (gameObject)
                {
                    PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
                    if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money)
                    {
                        purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost);
                    }
                }
            }
        }

        /// <summary>
        /// Overrideable OnEnter method from the original state. Always call base.OnEnter. Initialize values at runtime here.
        /// To perform the death behavior specified in OnImpactServer, destroyOnImpact must be set to true.
        /// This method already does that so long as base.OnEnter is invoked.
        /// </summary>
        public override void OnEnter()
        {
            VanillaDeathState originalState = Instantiate(typeof(VanillaDeathState)) as VanillaDeathState;
            deathDuration = originalState.deathDuration;
            initialExplosionEffect = originalState.initialExplosionEffect;
            deathExplosionEffect = originalState.deathExplosionEffect;
            initialSoundString = originalState.initialSoundString;
            deathSoundString = originalState.deathSoundString;
            deathEffectRadius = originalState.deathEffectRadius;
            forceAmount = originalState.forceAmount;
            deathDuration = originalState.deathDuration;
            destroyOnImpact = true;
            base.OnEnter();
        }

        /// <summary>
        /// Overrideable OnEnter method from the original state. Always call base.OnExit.
        /// </summary>
        public override void OnExit() => base.OnExit();

        /// <summary>
        /// Overrideable OnEnter method from the original state. Always call base.FixedUpdate.
        /// </summary>
        public override void FixedUpdate() => base.FixedUpdate();
    }
}