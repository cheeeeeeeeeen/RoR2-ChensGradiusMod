using RoR2;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class RedDeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = false;

        protected override InteractableSpawnCard GetInteractableSpawnCard => PsyDrone.iSpawnCard;
    }
}