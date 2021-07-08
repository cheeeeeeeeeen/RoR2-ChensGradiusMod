using Chen.GradiusMod.Items.GradiusOption;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaser : BaseState
    {
        private const uint FireSoundEffect = 108801625;

        private Transform modelTransform;
        private Transform aimOrigin;
        private Transform modelBaseTransform;
        private MirrorLaserController firstLC;
        private MirrorLaserController secondLC;
        private MirrorLaserController thirdLC;
        private ParticleSystem muzzleEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            modelBaseTransform = modelTransform.parent;
            aimOrigin = modelTransform.Find("AimOrigin");
            GameObject laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            firstLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            firstLC.direction = ComputeDirection(Vector3.forward);
            firstLC.spawnEffects = true;
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            secondLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            secondLC.direction = ComputeDirection(new Vector3(1, 0, 1));
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            thirdLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            thirdLC.direction = ComputeDirection(new Vector3(-1, 0, 1));
            firstLC.owner = secondLC.owner = thirdLC.owner = characterBody;
            muzzleEffect = aimOrigin.gameObject.GetComponentInChildren<ParticleSystem>();
            muzzleEffect.Play();
            AkSoundEngine.PostEvent(FireSoundEffect, gameObject);
            OptionsFire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (AllLasersComplete()) outer.SetNextStateToMain();
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

        private Vector3 ComputeDirection(Vector3 localDirection)
        {
            return (Util.QuaternionSafeLookRotation(modelBaseTransform.forward, modelBaseTransform.up) * localDirection).normalized;
        }

        private bool AllLasersComplete() => firstLC.complete && secondLC.complete && thirdLC.complete;

        private void OptionsFire()
        {
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, direction) =>
            {
                Transform oTransform = option.transform;
                GameObject laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, oTransform.position, Util.QuaternionSafeLookRotation(direction));
                MirrorLaserController laserController = laserObject.GetOrAddComponent<MirrorLaserController>();
                laserController.direction = direction;
                laserController.spawnEffects = true;
                laserController.owner = characterBody;
                laserController.damage = damageStat * GradiusOption.instance.damageMultiplier;
                laserController.speed = 1f;
                laserController.force = MirrorLaserController.BaseForce * GradiusOption.instance.damageMultiplier;
                ParticleSystem particleSystem;
                if (behavior.U.SafeCheck("muzzle.effect")) particleSystem = behavior.U["muzzle.effect"] as ParticleSystem;
                else
                {
                    GameObject muzzleEffect = Object.Instantiate(PsyDrone.mirrorLaserMuzzleEffect, oTransform);
                    behavior.U["muzzle.effect"] = particleSystem = muzzleEffect.GetComponent<ParticleSystem>();
                }
                particleSystem.Play();
            });
        }
    }
}