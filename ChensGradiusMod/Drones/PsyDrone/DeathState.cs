using RoR2;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = PsyDrone.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => PsyDrone.iSpawnCard;
    }
}