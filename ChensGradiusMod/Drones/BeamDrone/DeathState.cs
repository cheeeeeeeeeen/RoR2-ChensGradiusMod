using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = LaserDrone1.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => LaserDrone1.iSpawnCard;

        protected override void OnInteractableSpawn(GameObject spawnedObject)
        {
            base.OnInteractableSpawn(spawnedObject);
            spawnedObject.transform.position += spawnedObject.transform.up * .2f;
        }
    }
}