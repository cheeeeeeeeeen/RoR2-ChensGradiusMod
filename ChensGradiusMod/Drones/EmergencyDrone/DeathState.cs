using RoR2;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.EmergencyDrone
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEmergencyDrone"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.emergencyDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;
    }
}