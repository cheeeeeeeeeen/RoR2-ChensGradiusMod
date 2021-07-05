using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class Twins : MonoBehaviour
    {
        public GameObject twin;

        public Twins twinTwinComponent { get => twin.GetComponent<Twins>(); }

        private CharacterBody twinBody { get => twin.GetComponent<CharacterBody>(); }
        private HealthComponent twinHealthComponent { get => twinBody ? twinBody.healthComponent : null; }
        private CharacterBody body { get => GetComponent<CharacterBody>(); }
        private HealthComponent healthComponent { get => body ? body.healthComponent : null; }

        private void FixedUpdate()
        {
            if (twin && healthComponent && twinHealthComponent && healthComponent.alive && !twinHealthComponent.alive)
            {
                healthComponent.Suicide();
            }
        }
    }
}