using Chen.Helpers.UnityHelpers;
using EntityStates;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class SearchLaser : BaseState
    {
        private const uint FireSoundEffect = 4131085986;
        private const float Duration = 1.5f;
        private const float FireInterval = .3f;

        private Transform modelTransform;
        private Transform aimOrigin;
        private float fireTimer;
        private ParticleSystem muzzleEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            aimOrigin = modelTransform.Find("AimOrigin");
            fireTimer = 0f;
            muzzleEffect = aimOrigin.gameObject.GetComponent<ParticleSystem>();
            muzzleEffect.Play();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            fireTimer += Time.fixedDeltaTime;
            float computedInterval = FireInterval / attackSpeedStat;
            if (fireTimer >= computedInterval)
            {
                GameObject searchLaser = Object.Instantiate(PsyDrone.searchLaserPrefab, aimOrigin.position, aimOrigin.rotation);
                AkSoundEngine.PostEvent(FireSoundEffect, searchLaser);
                SearchLaserController laserController = searchLaser.GetOrAddComponent<SearchLaserController>();
                laserController.owner = characterBody;
                fireTimer -= computedInterval;
            }
            if (fixedAge >= Duration) outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            muzzleEffect.Stop();
            base.OnExit();
        }
    }
}