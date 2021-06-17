using RoR2;

namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = LaserDrone2.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => LaserDrone2.iSpawnCard;
    }
}