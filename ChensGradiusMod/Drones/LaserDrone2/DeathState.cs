using RoR2;

namespace Chen.GradiusMod.Drones.LaserDrone2
{
    internal class LaserDrone2DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return LaserDrone2.iSpawnCard;
        }
    }
}