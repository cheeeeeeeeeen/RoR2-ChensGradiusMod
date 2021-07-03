using RoR2;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class GreenDeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = PsyDrone.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => PsyDrone.iSpawnCard;
    }
}