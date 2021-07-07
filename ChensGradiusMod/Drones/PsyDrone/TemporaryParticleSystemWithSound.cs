using Chen.Helpers.UnityHelpers;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class TemporaryParticleSystemWithSound : TemporaryParticleSystem
    {
        private const uint ExplodeSoundEffectId = 3977903417;
        private const float TimeToDetonate = 5f;

        private float age;

        protected override void Awake()
        {
            base.Awake();
            age = 0f;
            AkSoundEngine.PostEvent(ExplodeSoundEffectId, gameObject);
        }

        protected override void FixedUpdate()
        {
            age += Time.fixedDeltaTime;
            if (age >= TimeToDetonate) detonate = true;
            base.FixedUpdate();
        }
    }
}