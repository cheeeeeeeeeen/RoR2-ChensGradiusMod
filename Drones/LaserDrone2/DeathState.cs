using RoR2;

namespace Chen.GradiusMod
{
    internal class LaserDrone2DeathState : DroneDeathState
    {
        private InteractableSpawnCard _interactable = null;

        protected override InteractableSpawnCard interactable
        {
            get
            {
                if (_interactable == null) _interactable = LaserDrone2.iSpawnCard;
                return interactable;
            }
        }
    }
}