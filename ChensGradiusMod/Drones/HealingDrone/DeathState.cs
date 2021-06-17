using RoR2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.HealingDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = generalCfg.healingDronesAreRepurchaseable;

        protected override InteractableSpawnCard GetInteractableSpawnCard => Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone2");
    }
}