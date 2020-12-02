using RoR2;

namespace Chen.GradiusMod
{
    internal class LaserDrone1DeathState : DroneDeathState
    {
        private InteractableSpawnCard _interactable = null;

        protected override InteractableSpawnCard interactable
        {
            get
            {
                if (_interactable == null) _interactable = LaserDrone1.iSpawnCard;
                return interactable;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            deathDuration = 1f;
        }
    }
}