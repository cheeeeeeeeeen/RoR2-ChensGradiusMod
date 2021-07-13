using RoR2;
using System;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.HealingDrone
{
    internal class DeathState : DroneDeathState
    {
        private static readonly Lazy<InteractableSpawnCard> _iSpawnCard =
            new Lazy<InteractableSpawnCard>(() => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone2"));

        protected override bool SpawnInteractable { get; set; } = generalCfg.healingDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => _iSpawnCard.Value;
    }
}