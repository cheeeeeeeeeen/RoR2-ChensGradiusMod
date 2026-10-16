using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Artifacts.Machines
{
    /// <summary>
    /// Marks a CharacterMaster as a drone that was given by the Artifact of Machines.
    /// Server-side only. Drones given to enemies are destroyed once their owner is gone.
    /// </summary>
    internal class MachinesDrone : MonoBehaviour
    {
        private const float CheckInterval = 1f;

        public bool cleanupWithOwner;

        private CharacterMaster master;
        private float stopwatch;

        private void Awake()
        {
            master = GetComponent<CharacterMaster>();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active || !cleanupWithOwner || !master || !master.minionOwnership) return;
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch < CheckInterval) return;
            stopwatch = 0f;

            CharacterMaster ownerMaster = master.minionOwnership.ownerMaster;
            bool ownerGone = !ownerMaster || (!ownerMaster.hasBody && ownerMaster.lostBodyToDeath);
            if (!ownerGone) return;

            CharacterBody body = master.GetBody();
            if (body && body.healthComponent && body.healthComponent.alive) body.healthComponent.Suicide();
            else if (!body) Destroy(gameObject);
        }
    }

    /// <summary>
    /// Marks a CharacterMaster as already processed by the Artifact of Machines.
    /// </summary>
    internal class MachinesProcessed : MonoBehaviour
    {
    }
}
