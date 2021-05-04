using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.GunnerTurret
{
    internal class DeathState : DroneDeathState
    {
        private InteractableSpawnCard spawnCard;

        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            if (!spawnCard) spawnCard = Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1");
            return spawnCard;
        }

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
            OnImpactServer(characterBody.transform.position - characterBody.transform.up * 1.2f);
            base.OnExit();
        }
    }
}