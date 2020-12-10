using RoR2;

namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return LaserDrone2.iSpawnCard;
        }
    }
}