using RoR2;

namespace Chen.GradiusMod.Drones.LaserDrone1
{
    internal class LaserDrone1DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return LaserDrone1.iSpawnCard;
        }
    }
}