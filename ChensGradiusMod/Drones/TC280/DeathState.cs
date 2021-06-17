using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using VanillaMegaDroneState = EntityStates.Drone.MegaDroneDeathState;

namespace Chen.GradiusMod.Drones.TC280
{
    internal class DeathState : DroneDeathState
    {
        private ChildLocator childLocator;
        private Transform modelTransform;

        protected override bool SpawnInteractable { get; set; } = generalCfg.megaDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMegaDrone");

        public override void OnEnter()
        {
            base.OnEnter();
            initialSoundString = VanillaMegaDroneState.initialSoundString;
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                if (NetworkServer.active && childLocator)
                {
                    childLocator.FindChild("LeftJet").gameObject.SetActive(false);
                    childLocator.FindChild("RightJet").gameObject.SetActive(false);
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && childLocator && VanillaMegaDroneState.initialEffect)
            {
                EffectManager.SpawnEffect(VanillaMegaDroneState.initialEffect, new EffectData
                {
                    origin = transform.position,
                    scale = VanillaMegaDroneState.initialEffectScale
                }, true);
            }
            ExplodeRigidbodiesOnStart explodeComponent = modelTransform.GetComponent<ExplodeRigidbodiesOnStart>();
            if (explodeComponent)
            {
                explodeComponent.force = VanillaMegaDroneState.explosionForce;
                explodeComponent.enabled = true;
            }
            base.OnExit();
        }
    }
}