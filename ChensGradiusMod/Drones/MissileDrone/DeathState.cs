using RoR2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.MissileDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = generalCfg.missileDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMissileDrone");
    }
}