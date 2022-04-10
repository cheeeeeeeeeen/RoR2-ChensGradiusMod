using RoR2;

namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = LaserDrone1.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => LaserDrone1.iSpawnCard;
    }
}