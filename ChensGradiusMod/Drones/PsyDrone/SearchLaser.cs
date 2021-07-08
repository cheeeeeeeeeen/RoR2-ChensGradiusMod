using Chen.GradiusMod.Items.GradiusOption;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
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
        private bool optionFire;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            aimOrigin = modelTransform.Find("AimOrigin");
            fireTimer = 0f;
            muzzleEffect = aimOrigin.gameObject.GetComponentInChildren<ParticleSystem>();
            muzzleEffect.Play();
            optionFire = false;
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, direction) =>
            {
                ParticleSystem particleSystem;
                if (behavior.U.SafeCheck("muzzle.effect")) particleSystem = behavior.U["muzzle.effect"] as ParticleSystem;
                else
                {
                    GameObject muzzleEffect = Object.Instantiate(PsyDrone.searchLaserMuzzleEffect, option.transform);
                    behavior.U["muzzle.effect"] = particleSystem = muzzleEffect.GetComponent<ParticleSystem>();
                }
                particleSystem.Play();
            });
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
                OptionsFire();
                optionFire = !optionFire;
            }

            if (fixedAge >= Duration) outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            muzzleEffect.Stop();
            GradiusOption.instance.FireForAllOptions(characterBody, (_o, behavior, _t, _d) =>
            {
                if (behavior.U.SafeCheck("muzzle.effect")) ((ParticleSystem)behavior.U["muzzle.effect"]).Stop();
            });
            base.OnExit();
        }

        private void OptionsFire()
        {
            if (!optionFire) return;
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, direction) =>
            {
                Transform oTransform = option.transform;
                GameObject searchLaser = Object.Instantiate(PsyDrone.searchLaserPrefab, oTransform.position, Util.QuaternionSafeLookRotation(direction));
                SearchLaserController laserController = searchLaser.GetOrAddComponent<SearchLaserController>();
                laserController.owner = characterBody;
                laserController.damage = damageStat * GradiusOption.instance.damageMultiplier;
                laserController.force = SearchLaserController.BaseForce * GradiusOption.instance.damageMultiplier;
                laserController.acceleration = .02f;
                laserController.smoothCurveRate = .003f;
                laserController.maximumSpeed = .8f;
            });
        }
    }
}