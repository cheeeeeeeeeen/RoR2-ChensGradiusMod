using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    /// <summary>
    /// Lives on the CharacterMaster of the green Psy Drone and refers back to the red drone that summoned it.
    /// </summary>
    internal class TwinsDeath : MonoBehaviour
    {
        public TwinsSpawn leader;

        private CharacterMaster master;

        private void Awake()
        {
            master = GetComponent<CharacterMaster>();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active || leader) return;
            // The red drone is gone without taking this twin with it. Remove this twin quietly so it does not linger alone.
            CharacterBody body = master ? master.GetBody() : null;
            if (body && body.healthComponent && body.healthComponent.alive) NetworkServer.Destroy(body.gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}
