using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static RoR2.GenericPickupController;

namespace Chen.GradiusMod.Drones.EquipmentDrone
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEquipmentDrone"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.equipmentDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;

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