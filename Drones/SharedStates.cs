using RoR2;
using VanillaDeathState = EntityStates.Drone.DeathState;

namespace Chen.GradiusMod
{
    /// <summary>
    /// An Entity State that should inherits from the original Drone.DeathState.
    /// The original code does not support custom spawn cards to be detected when dying so that the interactable can spawn again.
    /// This state will cater to custom drones so they are also able to spawn interactables upon death.
    /// </summary>
    public class DroneDeathState : VanillaDeathState
    {
        private InteractableSpawnCard interactable;

        public DroneDeathState(InteractableSpawnCard interactable)
        {
            this.interactable = interactable;
        }

        private void SpawnInteractable()
        {

        }
    }
}