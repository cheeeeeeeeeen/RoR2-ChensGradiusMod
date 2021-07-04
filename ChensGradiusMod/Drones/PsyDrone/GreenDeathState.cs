using RoR2;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class GreenDeathState : RedDeathState
    {
        protected override bool SpawnInteractable { get; set; } = PsyDrone.instance.canBeRepurchased;
    }
}