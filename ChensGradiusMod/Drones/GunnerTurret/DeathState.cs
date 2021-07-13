using RoR2;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.GunnerTurret
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.turretsAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;

        protected override void OnInteractableSpawn(GameObject spawnedObject)
        {
            base.OnInteractableSpawn(spawnedObject);
            spawnedObject.transform.rotation = transform.rotation;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            destroyOnImpact = false;
            hardCutoffDuration = maxFallDuration = bodyPreservationDuration = deathDuration = 1f;
        }

        public override void OnExit()
        {
            OnImpactServer(characterBody.transform.position);
            base.OnExit();
        }
    }
}