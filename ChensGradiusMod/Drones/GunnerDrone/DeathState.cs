using RoR2;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.GunnerDrone
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.gunnerDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;
    }
}