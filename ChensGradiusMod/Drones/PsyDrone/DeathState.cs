using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class DeathState : DroneDeathState
    {
        protected override bool SpawnInteractable { get; set; } = PsyDrone.instance.canBeRepurchased;

        protected override InteractableSpawnCard GetInteractableSpawnCard => PsyDrone.iSpawnCard;

        public override void OnImpactServer(Vector3 contactPoint)
        {
            // Both twins die together, but only one broken drone should be left behind for the pair.
            // The claim is stored on the dying bodies since the masters can be destroyed while the bodies are still falling.
            BrokenDroneClaim claim = GetComponent<BrokenDroneClaim>();
            if (!claim)
            {
                gameObject.AddComponent<BrokenDroneClaim>().claimant = gameObject;
                CharacterBody twinBody = TwinsSpawn.FindTwinBody(characterBody ? characterBody.master : null);
                if (twinBody && !twinBody.GetComponent<BrokenDroneClaim>()) twinBody.gameObject.AddComponent<BrokenDroneClaim>().claimant = gameObject;
            }
            else if (claim.claimant != gameObject) return;
            base.OnImpactServer(contactPoint);
        }
    }

    internal class BrokenDroneClaim : MonoBehaviour
    {
        public GameObject claimant;
    }
}
