using RoR2;

namespace Chen.GradiusMod
{
    internal class LaserDrone1DeathState : DroneDeathState
    {
        protected override InteractableSpawnCard GetInteractableSpawnCard()
        {
            return LaserDrone1.iSpawnCard;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            deathDuration = 1f;
        }
    }
}