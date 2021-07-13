using RoR2;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.MissileDrone
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMissileDrone"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.missileDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;
    }
}