using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static RoR2.GenericPickupController;

namespace Chen.GradiusMod.Drones.Vanilla
{
    internal class EquipmentDroneDeathState : DroneDeathState
    {
        private InteractableSpawnCard spawnCard;

        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            if (!spawnCard) spawnCard = Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEquipmentDrone");
            return spawnCard;
        }

        protected override void OnInteractableSpawn(GameObject spawnedObject)
        {
            base.OnInteractableSpawn(spawnedObject);
            if (NetworkServer.active && Util.CheckRoll(generalCfg.dropEquipFromDroneChance, characterBody.master))
            {
                PickupIndex equipIndex = PickupCatalog.FindPickupIndex(characterBody.inventory.currentEquipmentIndex);
                Vector3 spawnPosition = characterBody.corePosition;
                CreatePickupInfo pickupInfo = new CreatePickupInfo
                {
                    position = spawnPosition,
                    rotation = Quaternion.identity,
                    pickupIndex = equipIndex
                };
                CreatePickup(pickupInfo);
            }
        }
    }
}