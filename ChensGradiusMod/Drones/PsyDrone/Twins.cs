using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class Twins : MonoBehaviour
    {
        public GameObject twin;

        private CharacterBody twinBody { get => twin.GetComponent<CharacterBody>(); }
        private HealthComponent twinHealthComponent { get => twinBody ? twinBody.healthComponent : null; }
        private CharacterBody body { get => GetComponent<CharacterBody>(); }
        private HealthComponent healthComponent { get => body ? body.healthComponent : null; }

        private void FixedUpdate()
        {
            if (!twin) Destroy(this);
            if (body && twinBody && twinHealthComponent && !twinHealthComponent.alive)
            {
                healthComponent.Suicide();
                Destroy(this);
            }
        }
    }
}