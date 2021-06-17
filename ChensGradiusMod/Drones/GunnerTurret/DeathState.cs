using RoR2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.GunnerTurret
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = generalCfg.turretsAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1");

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