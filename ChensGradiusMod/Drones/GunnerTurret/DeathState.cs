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

        public override void OnEnter()
        {
            base.OnEnter();
            destroyOnImpact = false;
        }

        public override void OnExit()
        {
            OnImpactServer(characterBody.transform.position - characterBody.transform.up * .9f);
            base.OnExit();
        }
    }
}