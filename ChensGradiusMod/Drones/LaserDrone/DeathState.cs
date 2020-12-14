using RoR2;
using UnityEngine;

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