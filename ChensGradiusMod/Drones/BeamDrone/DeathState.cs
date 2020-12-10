using RoR2;

namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return LaserDrone1.iSpawnCard;
        }
    }
}