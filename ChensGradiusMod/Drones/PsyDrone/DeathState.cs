using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = PsyDrone.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => PsyDrone.iSpawnCard;

        public override void OnImpactServer(Vector3 contactPoint)
        {
            Twins twinComponent = GetComponent<Twins>();
            if (twinComponent)
            {
                base.OnImpactServer(contactPoint);
                Destroy(twinComponent);
                Destroy(twinComponent.twinTwinComponent);
            }
        }
    }
}