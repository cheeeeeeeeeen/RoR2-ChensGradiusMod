using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod
{
    internal class Turret1DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return Resources.Load<InteractableSpawnCard>("Assets/Resources/spawncards/interactablespawncard/iscBrokenTurret1.asset");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && fixedAge > deathDuration)
            {
                OnImpactServer(transform.position);
            }
        }
    }
}